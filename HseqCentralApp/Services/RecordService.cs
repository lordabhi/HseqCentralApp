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
        private ApplicationDbContext db = new ApplicationDbContext();

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

            }
            else if (recordType.Equals(RecordType.FIS))
            {
                ViewBag.RecordType = RecordType.FIS;
                ViewBag.NcrState = NcrState.New;
            }

            return ViewBag;
        }

        public dynamic PopulateRecordTypeLinked(HseqRecord record, RecordType recordType)
        {
            ViewBag.EnteredBy = record.EnteredBy;
            ViewBag.ReportedBy = record.ReportedBy;
            ViewBag.QualityCoordinator = record.QualityCoordinator;

            if (recordType.Equals(RecordType.NCR))
            {
                ViewBag.RecordType = RecordType.NCR;
                ViewBag.NcrState = NcrState.New;
            }
            else if (recordType.Equals(RecordType.FIS))
            {
                ViewBag.RecordType = RecordType.FIS;
                ViewBag.NcrState = NcrState.New;
            }

            return ViewBag;
        }

        public ApplicationUser GetCurrentUser() {

            ApplicationUser currentUser = db.Users.Where(m => m.Email == HttpContext.Current.User.Identity.Name).First();
            return currentUser;
        
        }

        internal HseqRecord GetSourceRecord(int recordId, string recordSource, ApplicationDbContext db)
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

        internal int GetNextCaseNumber(ApplicationDbContext db)
        {
            int caseNo = 1;

            IList<HseqCaseFile> hseqCaseFilesList = db.HseqCaseFiles.ToList();

            if (hseqCaseFilesList != null && hseqCaseFilesList.LongCount() > 0)
            {

                caseNo = hseqCaseFilesList.Max(p => p.CaseNo) + 1;

            }
            return caseNo;
        }

        public HseqRecord CreateCaseFile(HseqRecord record, out int caseNo, out HseqCaseFile hseqCaseFile, ApplicationDbContext db)
        {
            caseNo = GetNextCaseNumber(db);

            hseqCaseFile = new HseqCaseFile();

            hseqCaseFile.CaseNo = caseNo;

            db.HseqCaseFiles.Add(hseqCaseFile);

            record.HseqCaseFile = hseqCaseFile;
            record.HseqCaseFileID = hseqCaseFile.HseqCaseFileID;

            hseqCaseFile.HseqRecords.Add(record);

            return record;
        }

        public HseqRecord PopulateLinkedRecordDefaults(String recordSource, HseqRecord linkedRecord, HseqRecord ncr, TempDataDictionary TempData)
        {
            ncr.HseqRecordID = linkedRecord.HseqRecordID;

            ncr.EnteredBy = linkedRecord.EnteredBy;
            ncr.ReportedBy = linkedRecord.ReportedBy;
            ncr.QualityCoordinator = linkedRecord.QualityCoordinator;

            TempData["recordId"] = linkedRecord.HseqRecordID;
            TempData["recordSource"] = recordSource;

            linkedRecord.LinkedRecords.Add(ncr);

            return ncr;
        }

        public HseqRecord CreateLinkingForRecords(Ncr ncr, TempDataDictionary TempData, ApplicationDbContext db)
        {
            if (TempData["recordId"] != null)
            {
                var recordId = (int)TempData["recordId"];
                var recordSource = (string)TempData["recordSource"];

                HseqRecord linkedRecord = GetSourceRecord(recordId, recordSource, db);

                if (linkedRecord != null)
                {
                    ncr.AlfrescoNoderef = linkedRecord.AlfrescoNoderef;
                    ncr.HseqCaseFileID = linkedRecord.HseqCaseFileID;
                    ncr.HseqCaseFile = linkedRecord.HseqCaseFile;

                }

                ncr.LinkedRecords.Add(linkedRecord);
                linkedRecord.LinkedRecords.Add(ncr);

                TempData["recordId"] = null;
                TempData["recordSource"] = null;
            }

            db.SaveChanges();

            return ncr;
        }
    }
}