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

                context.DiscrepancyTypes.Add(temp);
            }
            //context.SaveChanges();

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

                context.BusinessAreas.Add(temp);
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

                context.DispositionTypes.Add(temp);
            }

            context.SaveChanges();


        }
    }
}