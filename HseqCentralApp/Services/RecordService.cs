﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using HseqCentralApp.Models;
using HseqCentralApp.Helpers;

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
            //ViewBag.QualityCoordinator = ConfigurationManager.AppSettings.Get("QualityCoordinator");
            
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
            else if (recordType.Equals(RecordType.CAR))
            {
                ViewBag.RecordType = RecordType.CAR;
                ViewBag.NcrState = NcrState.New;
            }
            else if (recordType.Equals(RecordType.PAR))
            {
                ViewBag.RecordType = RecordType.PAR;
                ViewBag.NcrState = NcrState.New;
            }
            return ViewBag;
        }

        public HseqRecord PopulateRecordTypeDefaults(RecordType recordType, HseqRecord hseqRecord)
        {
            hseqRecord.EnteredBy = HttpContext.Current.User.Identity.Name;
            hseqRecord.ReportedBy = currentUser.FirstName + " " + currentUser.LastName + " , " + currentUser.Department;

            //Retrieve the Quality Coordinator from the custom file
            //ViewBag.QualityCoordinator = ConfigurationManager.AppSettings.Get("QualityCoordinator");

            if (recordType.Equals(RecordType.NCR))
            {
                hseqRecord.RecordType = RecordType.NCR;
            }
            else if (recordType.Equals(RecordType.FIS))
            {
                hseqRecord.RecordType = RecordType.FIS;
            }
            else if (recordType.Equals(RecordType.CAR))
            {
                hseqRecord.RecordType = RecordType.CAR;
            }
            else if (recordType.Equals(RecordType.PAR))
            {
                hseqRecord.RecordType = RecordType.PAR;
            }
            return hseqRecord;
        }

        
        public ApplicationUser GetCurrentUser() {

            ApplicationUser currentUser = db.Users.Where(m => m.Email == HttpContext.Current.User.Identity.Name).First();
            return currentUser;
        
        }

        public HseqUser GetCurrentApplicationUser()
        {

            ApplicationUser currentUser = GetCurrentUser();
            HseqUser hseqUser = db.HseqUsers.Where(a => a.UserID == currentUser.Id).First();
            return hseqUser;
        }


        internal int GetNextCaseNumber2(ApplicationDbContext db)
        {
            int caseNo = 1;

            IList<HseqCaseFile> hseqCaseFilesList = db.HseqCaseFiles.ToList();

            if (hseqCaseFilesList != null && hseqCaseFilesList.LongCount() > 0)
            {

                //caseNo = hseqCaseFilesList.Max(p => p.CaseNo) + 1;

            }
            return caseNo;
        }

        internal string GetNextCaseNumber(ApplicationDbContext db)
        {

            int year = DateTime.Now.Year;
            int lastTwoYearDigits = year % 100;

            string nextCaseNbr = "";
            string currentMaxCaseNbr="";

            IList<HseqCaseFile> hseqCaseFilesList = db.HseqCaseFiles.ToList();

            if (hseqCaseFilesList == null || hseqCaseFilesList.LongCount() == 0)
            {
                //nextCaseNbr = lastTwoYearDigits + "-" + "1000";
                //nextCaseNbr = String.Format("{0}-{1}", lastTwoYearDigits, "1000");
                nextCaseNbr = ((lastTwoYearDigits * 10000) + 1000).ToString();
            }

            else if (hseqCaseFilesList != null && hseqCaseFilesList.LongCount() > 0)
            {
                currentMaxCaseNbr = hseqCaseFilesList.Max(p => p.CaseNo);

                if (currentMaxCaseNbr.StartsWith(lastTwoYearDigits.ToString()))
                {
                    nextCaseNbr = (Convert.ToInt32(currentMaxCaseNbr) + 1).ToString();
                }
                else if (lastTwoYearDigits == 0)
                {
                    //nextCaseNbr = String.Format("{0}-{1}",lastTwoYearDigits ,"1000");
                    //nextCaseNbr = ((Convert.ToInt32(year.ToString().Substring(0, 2)) * 10000) + 1000).ToString();
                    nextCaseNbr = ((lastTwoYearDigits * 10000) + 1000).ToString();
                }
            }

            return nextCaseNbr;
        }


        public HseqRecord CreateCaseFile(HseqRecord record, out string caseNo, out HseqCaseFile hseqCaseFile, ApplicationDbContext db)
        {
            caseNo = GetNextCaseNumber(db);

            record.CaseNo = caseNo;
            record.RecordNo = caseNo;

            hseqCaseFile = new HseqCaseFile() {
                CaseNo = caseNo
            };

            db.HseqCaseFiles.Add(hseqCaseFile);

            db.SaveChanges();
            record.HseqCaseFile = hseqCaseFile;
            record.HseqCaseFileID = hseqCaseFile.HseqCaseFileID;

            hseqCaseFile.HseqRecords.Add(record);

            return record;
        }

        public HseqRecord CreateCaseFile(HseqRecord record, out HseqCaseFile hseqCaseFile, ApplicationDbContext db)
        {

            hseqCaseFile = new HseqCaseFile()
            {
                CaseNo = record.CaseNo
            };

            db.HseqCaseFiles.Add(hseqCaseFile);

            db.SaveChanges();
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
            //ncr.QualityCoordinator = linkedRecord.QualityCoordinator;

            TempData["recordId"] = linkedRecord.HseqRecordID;
            TempData["recordSource"] = recordSource;

            linkedRecord.LinkedRecords.Add(ncr);

            return ncr;
        }

        //public HseqRecord CreateLinkingForRecords(Ncr ncr, TempDataDictionary TempData, ApplicationDbContext db)
        //{
        //    if (TempData["recordId"] != null)
        //    {
        //        var recordId = (int)TempData["recordId"];
        //        var recordSource = (string)TempData["recordSource"];

        //        //HseqRecord linkedRecord = GetSourceRecord(recordId, recordSource, db);
        //        HseqRecord linkedRecord = null;
        //        if (linkedRecord != null)
        //        {
        //            ncr.AlfrescoNoderef = linkedRecord.AlfrescoNoderef;
        //            ncr.HseqCaseFileID = linkedRecord.HseqCaseFileID;
        //            ncr.HseqCaseFile = linkedRecord.HseqCaseFile;

        //        }

        //        ncr.LinkedRecords.Add(linkedRecord);
        //        linkedRecord.LinkedRecords.Add(ncr);

        //        TempData["recordId"] = null;
        //        TempData["recordSource"] = null;
        //    }

        //    db.SaveChanges();

        //    return ncr;
        //}


    }
}