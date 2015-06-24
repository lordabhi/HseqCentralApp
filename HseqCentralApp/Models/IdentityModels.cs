using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HseqCentralApp.Models
{

    // You will not likely need to customize there, but it is necessary/easier to create our own 
    // project-specific implementations, so here they are:
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }

    // Must be expressed in terms of our custom Role and other types:
    public class ApplicationUser : IdentityUser<string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();

            // Add any custom User properties/code here
        }

        [Display(Name = "First Name")]
        public String FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        [Display(Name = "Department")]
        public String Department { get; set; }
    
        public string FullName
        {
            get {
                return this.FirstName + " " + this.LastName;
            }

        }
    
        public async Task<ClaimsIdentity>
            GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager
                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }


    // Must be expressed in terms of our custom UserRole:
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }

        // Add any custom Role properties/code here
    }


    // Must be expressed in terms of our custom types:
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext(): base("HseqCentralAppContext")
        {
        }

        //Abhi///
        public System.Data.Entity.DbSet<HseqCentralApp.Models.HseqCaseFile> HseqCaseFiles { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.Ncr> NcrRecords { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.Fis> FisRecords { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.DiscrepancyType> DiscrepancyTypes { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.HseqRecord> HseqRecords { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.BusinessArea> BusinessAreas { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.DispositionType> DispositionTypes { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.ApproverDisposition> ApproverDispositions { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.Car> CarRecords { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.Par> ParRecords { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.FisCategory> FisCategories { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.FisCode> FisCodes { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.HseqApprovalRequest> HseqApprovalRequests { get; set; }

        public System.Data.Entity.DbSet<HseqCentralApp.Models.HseqUser> HseqUsers { get; set; }


        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // Add the ApplicationGroups property:
        public virtual IDbSet<ApplicationGroup> ApplicationGroups { get; set; }

        // Override OnModelsCreating:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationUserGroup>((ApplicationGroup g) => g.ApplicationUsers)
                .WithRequired().HasForeignKey<string>((ApplicationUserGroup ag) => ag.ApplicationGroupId);
            modelBuilder.Entity<ApplicationUserGroup>()
                .HasKey((ApplicationUserGroup r) =>
                    new
                    {
                        ApplicationUserId = r.ApplicationUserId,
                        ApplicationGroupId = r.ApplicationGroupId
                    }).ToTable("ApplicationUserGroups");

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany<ApplicationGroupRole>((ApplicationGroup g) => g.ApplicationRoles)
                .WithRequired().HasForeignKey<string>((ApplicationGroupRole ap) => ap.ApplicationGroupId);
            modelBuilder.Entity<ApplicationGroupRole>().HasKey((ApplicationGroupRole gr) =>
                new
                {
                    ApplicationRoleId = gr.ApplicationRoleId,
                    ApplicationGroupId = gr.ApplicationGroupId
                }).ToTable("ApplicationGroupRoles");

            /////////////////////////////////
            modelBuilder.Entity<HseqRecord>().HasMany(m => m.LinkedRecords).WithMany();

            //one-to-many 
            modelBuilder.Entity<HseqRecord>()
                        .HasOptional<HseqCaseFile>(h => h.HseqCaseFile)
                        .WithMany(h => h.HseqRecords)
                        .HasForeignKey(h => h.HseqCaseFileID);

            //modelBuilder.Entity<HseqCaseFile>()
            //                        .HasMany<HseqRecord>(s => s.HseqRecords)
            //                        .WithRequired(s => s.HseqCaseFile)
            //                        .HasForeignKey(s => s.HseqCaseFileID);
            /////////////////////////////////////////////////////////////////////


            modelBuilder.Entity<Ncr>()
                .HasRequired(c => c.DetectedInArea)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Ncr>()
                .HasRequired(s => s.ResponsibleArea)
                .WithMany()
                .WillCascadeOnDelete(false);

            ///////////////////////////////////////////////////////////////

            modelBuilder.Entity<Delegatable>()
                .HasRequired(o => o.Owner)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Delegatable>()
                .HasRequired(d => d.Assignee)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HseqRecord>()
                .HasRequired(d => d.Coordinator)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HseqRecord>()
                .HasOptional(d => d.Coordinator)
                .WithMany();
        }


        //public System.Data.Entity.DbSet<HseqCentralApp.Models.User> Users { get; set; }


    }


    // Most likely won't need to customize these either, but they were needed because we implemented
    // custom versions of all the other types:
    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>, IUserStore<ApplicationUser, string>, IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole>, IQueryableRoleStore<ApplicationRole, string>, IRoleStore<ApplicationRole, string>, IDisposable
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationGroup
    {
        public ApplicationGroup()
        {
            this.Id = Guid.NewGuid().ToString();
            this.ApplicationRoles = new List<ApplicationGroupRole>();
            this.ApplicationUsers = new List<ApplicationUserGroup>();
        }

        public ApplicationGroup(string name)
            : this()
        {
            this.Name = name;
        }

        public ApplicationGroup(string name, string description)
            : this(name)
        {
            this.Description = description;
        }

        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ApplicationGroupRole> ApplicationRoles { get; set; }
        public virtual ICollection<ApplicationUserGroup> ApplicationUsers { get; set; }
    }


    public class ApplicationUserGroup
    {
        public string ApplicationUserId { get; set; }
        public string ApplicationGroupId { get; set; }
    }

    public class ApplicationGroupRole
    {
        public string ApplicationGroupId { get; set; }
        public string ApplicationRoleId { get; set; }
    }
}