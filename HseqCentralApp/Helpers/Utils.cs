using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.Helpers
{
    public sealed class Utils
    {
        private Utils() { }

        private static ApplicationDbContext db = new ApplicationDbContext();

        public static int CurrentCaseNumber
        {
            get { return int.Parse(HttpContext.Current.Session["CurrentCaseNumber"].ToString()); }
            set { HttpContext.Current.Session["CurrentCaseNumber"] = 0; }
        }

        public static ApplicationUser GetCurrentUser()
        {

            ApplicationUser currentUser = db.Users.Where(m => m.Email == HttpContext.Current.User.Identity.Name).First();
            return currentUser;

        }

        public static HseqUser GetCurrentApplicationUser()
        {

            ApplicationUser currentUser = GetCurrentUser();
            HseqUser hseqUser = db.HseqUsers.Where(a => a.UserID == currentUser.Id).First();
            return hseqUser;
        }

        public static HseqUser GetCurrentApplicationUser(ApplicationDbContext db)
        {

            ApplicationUser currentUser = GetCurrentUser();
            HseqUser hseqUser = db.HseqUsers.Where(a => a.UserID == currentUser.Id).First();
            return hseqUser;
        }

        public static String CaseNumberForDisplay(string caseNbrStr) {

            //string caseNbrStr = caseNbr.ToString();
            if (string.IsNullOrEmpty(caseNbrStr)) { 
                return "";
            }
            if (caseNbrStr.Length > 2)
            {
                return (caseNbrStr.Insert(2, "-"));
            }

            return caseNbrStr;
        }

        ////////////////////////////////////////////////////////////////////////////////

        public static IEnumerable<DiscrepancyType> DiscrepancyTypes()
        {
            return db.DiscrepancyTypes.ToList();
        }

        public static IEnumerable<DispositionType> DispositionTypes()
        {
            return db.DispositionTypes.ToList();
        }

        public static IEnumerable<NcrSource> NcrSources()
        {

            return Enum.GetValues(typeof(NcrSource)).Cast<NcrSource>();
            //Enum.GetValues(typeof(SomeEnum))
            //    .Cast<SomeEnum>()
            //    .Select(v => v.ToString())
            //    .ToList();
        }

        public static IEnumerable<NcrState> NcrStates()
        {

            return Enum.GetValues(typeof(NcrState)).Cast<NcrState>();
        }

        public static IEnumerable<BusinessArea> BusinessAreas()
        {
            return db.BusinessAreas.ToList();
        }

        public static IEnumerable<HseqUser> AppUsers()
        {
            return db.HseqUsers.ToList();
        }

        public static IEnumerable<BusinessArea> ResponsibleAreas()
        {
            return db.BusinessAreas.ToList();
        }
    }
}