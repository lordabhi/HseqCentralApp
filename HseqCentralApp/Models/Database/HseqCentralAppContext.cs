using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class HseqCentralAppContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public HseqCentralAppContext() : base("name=HseqCentralAppContext")
        {
        }

        //public System.Data.Entity.DbSet<HseqCentralApp.Models.HseqCaseFile> HseqCaseFiles { get; set; }

        //public System.Data.Entity.DbSet<HseqCentralApp.Models.Ncr> NcrRecords { get; set; }

        //public System.Data.Entity.DbSet<HseqCentralApp.Models.Fis> FisRecords { get; set; }

        //public System.Data.Entity.DbSet<HseqCentralApp.Models.DiscrepancyType> DiscrepancyTypes { get; set; }

        //public System.Data.Entity.DbSet<HseqCentralApp.Models.HseqRecord> HseqRecords { get; set; }

        //public System.Data.Entity.DbSet<HseqCentralApp.Models.BusinessArea> BusinessAreas { get; set; }

        //public System.Data.Entity.DbSet<HseqCentralApp.Models.DispositionType> DispositionTypes { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<HseqRecord>().HasMany(m => m.LinkedRecords).WithMany();

        //    //one-to-many 
        //    modelBuilder.Entity<HseqRecord>()
        //                .HasOptional<HseqCaseFile>(h => h.HseqCaseFile)
        //                .WithMany(h => h.HseqRecords)
        //                .HasForeignKey(h => h.HseqCaseFileID);

        //    //modelBuilder.Entity<HseqCaseFile>()
        //    //                        .HasMany<HseqRecord>(s => s.HseqRecords)
        //    //                        .WithRequired(s => s.HseqCaseFile)
        //    //                        .HasForeignKey(s => s.HseqCaseFileID);

        //}
    
    }
}
