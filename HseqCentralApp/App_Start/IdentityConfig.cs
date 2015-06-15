using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser, ApplicationRole, string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }


    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }


    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            InitializeApplicationData(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name, EmailConfirmed = true};
                user.FirstName = "FAdmin";
                user.LastName = "LAdmin";
                user.Department = "Information Technology";

                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);

            }

            var groupManager = new ApplicationGroupManager();
            var newGroup = new ApplicationGroup("SuperAdmins", "Full Access to All");

            groupManager.CreateGroup(newGroup);
            groupManager.SetUserGroups(user.Id, new string[] { newGroup.Id });
            groupManager.SetGroupRoles(newGroup.Id, new string[] { role.Name });
        }

        public static void InitializeApplicationData(ApplicationDbContext db)
        {

            //Discrepancy Types
            var discrepancyTypes = new List<DiscrepancyType>{

                new DiscrepancyType{Name = "OS&D"},
                new DiscrepancyType{Name = "Wrong Material"},
                new DiscrepancyType{Name = "Testing Failure"},
                new DiscrepancyType{Name = "Weld Quality"},
                new DiscrepancyType{Name = "Fitting Error"}
            };

            foreach (var temp in discrepancyTypes)
            {

                db.DiscrepancyTypes.Add(temp);
            }


            //Business Areas
            var businessAreas = new List<BusinessArea>{

                new BusinessArea{Name = "Project Planning"},
                new BusinessArea{Name = "Project Management"},
                new BusinessArea{Name = "Project Cost Control"},
                new BusinessArea{Name = "Drafting"},
                new BusinessArea{Name = "Engineering"},
                new BusinessArea{Name = "Shop Detailing"},
                new BusinessArea{Name = "Shop Fitting"},
                new BusinessArea{Name = "Shop Welding"},
                new BusinessArea{Name = "Shop Paint"},
                new BusinessArea{Name = "Shop Hse"},
                new BusinessArea{Name = "Shop Qc"},
                new BusinessArea{Name = "Shipping/Receiving"},
                new BusinessArea{Name = "Operations Management"},
                new BusinessArea{Name = "Accounts Receivable"},
                new BusinessArea{Name = "Accounts Payable"},
                new BusinessArea{Name = "Payroll"},
                new BusinessArea{Name = "Field Admin"},
                new BusinessArea{Name = "Field Workface"},
                new BusinessArea{Name = "Field Hse"},
                new BusinessArea{Name = "Field Qc"},
                new BusinessArea{Name = "Information Systems"}
            };

            foreach (var temp in businessAreas)
            {

                db.BusinessAreas.Add(temp);
            }

            var dispositionTypes = new List<DispositionType>{

                new DispositionType{Name = "Rework"},
                new DispositionType{Name = "Use As Is"},
                new DispositionType{Name = "Scrap"},
                new DispositionType{Name = "Remake"},
                new DispositionType{Name = "Return To Vendor"}
            };

            foreach (var temp in dispositionTypes)
            {

                db.DispositionTypes.Add(temp);
            }

            //FIS Categories and Codes
            var fisCategories = new List<FisCategory>
            {
            };

            var category1 = new FisCategory { Name = "Category 1" };
            fisCategories.Add(category1);

            var category2 = new FisCategory { Name = "Category 2" };
            fisCategories.Add(category2);

            var category3 = new FisCategory { Name = "Category 3" };
            fisCategories.Add(category3);

            foreach (var temp in fisCategories)
            {
                db.FisCategories.Add(temp);
            }

            var fisCategoryCodes = new List<FisCode>{
            
                    new FisCode{FisCategory = category1, CodeName = "Code1"},
                    new FisCode{FisCategory = category1, CodeName = "Code2"},
                    new FisCode{FisCategory = category1, CodeName = "Code3"},
                    new FisCode{FisCategory = category2, CodeName = "Code4"},
                    new FisCode{FisCategory = category2, CodeName = "Code5"},
                    new FisCode{FisCategory = category2, CodeName = "Code6"},
                    new FisCode{FisCategory = category2, CodeName = "Code7"},
                    new FisCode{FisCategory = category3, CodeName = "Code8"},
                    new FisCode{FisCategory = category3, CodeName = "Code9"}
            };

            foreach (var temp in fisCategoryCodes)
            {
                db.FisCodes.Add(temp);
            }

            db.SaveChanges();

        }
    }
}
