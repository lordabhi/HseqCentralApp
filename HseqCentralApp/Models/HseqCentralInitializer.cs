using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class HseqCentralInitializer : DropCreateDatabaseIfModelChanges<HseqCentralAppContext>
    {
        protected override void Seed(HseqCentralAppContext context)
        {
            var discrepancyTypes = new List<DiscrepancyType>{

                new DiscrepancyType{Name = "OS&D"},
                new DiscrepancyType{Name = "Wrong Material"},
                new DiscrepancyType{Name = "Testing Failure"},
                new DiscrepancyType{Name = "Weld Quality"},
                new DiscrepancyType{Name = "Fitting Error"}
            };

            foreach (var temp in discrepancyTypes)
            {

                context.DiscrepancyTypes.Add(temp);
            }
            context.SaveChanges();

        }
    }
}