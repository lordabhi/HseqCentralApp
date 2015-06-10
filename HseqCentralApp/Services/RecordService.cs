using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using HseqCentralApp.Models;

using System.Security.Principal;
using System.Configuration;

namespace HseqCentralApp.Services
{
    public class RecordService
    {
        //private HseqCentralAppContext db = new HseqCentralAppContext();
        private ApplicationDbContext appDb = new ApplicationDbContext();

        private ApplicationUser currentUser;
        private dynamic ViewBag;

        public RecordService() {

            ViewBag = new System.Dynamic.ExpandoObject();
            currentUser = GetCurrentUser();
        }

        public dynamic PopulateRecordTypeDefaults(RecordType recordType)
        {
            ViewBag.EnteredBy = HttpContext.Current.User.Identity.Name;
            ViewBag.ReportedBy = currentUser.FirstName + " " + currentUser.LastName + " , " + currentUser.Department;

            //Retrieve the Quality Coordinator from the custom file
            ViewBag.QualityCoordinator = ConfigurationManager.AppSettings.Get("QualityCoordinator");
            
            if (recordType.Equals(RecordType.NCR))
            {
                ViewBag.RecordType = RecordType.NCR;
                ViewBag.NcrState = NcrState.New;
                //ViewBag.Status = NcrState.New;

            }else{
                ViewBag.RecordType = RecordType.FIS;
                ViewBag.NcrState = NcrState.New;
                //ViewBag.Status = NcrState.New;
            }

            return ViewBag;
        }

        public ApplicationUser GetCurrentUser() {

            ApplicationUser currentUser = appDb.Users.Where(m => m.Email == HttpContext.Current.User.Identity.Name).First();
            return currentUser;
        
        }

        internal HseqRecord GetSourceRecord(int recordId, string recordSource, HseqCentralAppContext db)
        {

            HseqRecord linkedRecord = null;

            if (recordSource.Equals(RecordType.FIS.ToString()))
            {
                linkedRecord = db.FisRecords.Find(recordId);
            }
            else if (recordSource.Equals(RecordType.NCR.ToString()))
            {
                linkedRecord = db.NcrRecords.Find(recordId);
            }

            return linkedRecord;
        }

        internal int GetNextCaseNumber(HseqCentralAppContext db)
        {
            int caseNo = 1;

            IList<HseqCaseFile> hseqCaseFilesList = db.HseqCaseFiles.ToList();

            if (hseqCaseFilesList != null && hseqCaseFilesList.LongCount() > 0)
            {

                caseNo = hseqCaseFilesList.Max(p => p.CaseNo) + 1;

            }
            return caseNo;
        }

    }
}