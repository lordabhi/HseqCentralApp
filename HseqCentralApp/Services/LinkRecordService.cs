using HseqCentralApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Services
{
    public class LinkRecordService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private dynamic ViewBag;

        private RecordService _RecordService;

        public LinkRecordService() {
            ViewBag = new System.Dynamic.ExpandoObject();
            _RecordService = new RecordService();
        }

        public HseqRecord LinkRecord(int recordId, String recordSource, RecordType recordType, ApplicationDbContext db)
        {
            HseqRecord linkedRecord = GetSourceRecord(recordId, recordSource, db);

            //var defaults = PopulateRecordTypeLinked(linkedRecord, RecordType.NCR);

            HseqRecord record = null;
            
            if (recordType == RecordType.NCR) {
                 record = new Ncr(linkedRecord);
                 record.RecordType = RecordType.NCR;
            }
            else if (recordType == RecordType.FIS)
            {
                record = new Fis(linkedRecord);
                record.RecordType = RecordType.FIS;
            }
            else if (recordType == RecordType.CAR)
            {
                record = new Car(linkedRecord);
                record.RecordType = RecordType.CAR;
            }
            else if (recordType == RecordType.PAR)
            {
                record = new Par(linkedRecord);
                record.RecordType = RecordType.PAR;
            }            
            
            record.HseqRecordID = linkedRecord.HseqRecordID;

            linkedRecord.LinkedRecords.Add(record);
            return record;
        }

        internal HseqRecord GetSourceRecord(int recordId, string recordSource, ApplicationDbContext db)
        {

            HseqRecord linkedRecord = null;

            if (recordSource.Equals(RecordType.NCR.ToString()))
            {
                linkedRecord = db.NcrRecords.Find(recordId);
            }
            else if (recordSource.Equals(RecordType.FIS.ToString()))
            {
                linkedRecord = db.FisRecords.Find(recordId);
            }
            else if (recordSource.Equals(RecordType.CAR.ToString()))
            {
                linkedRecord = db.CarRecords.Find(recordId);
            }
            else if (recordSource.Equals(RecordType.PAR.ToString()))
            {
                linkedRecord = db.ParRecords.Find(recordId);
            }
            return linkedRecord;
        }

        internal HseqRecord CreateLinkRecord(HseqRecord record, int recordId, string recordSource, RecordType recordType, ApplicationDbContext db)
        {
            HseqRecord linkedRecord = GetSourceRecord(recordId, recordSource, db);

            if (linkedRecord != null)
            {
                record.AlfrescoNoderef = linkedRecord.AlfrescoNoderef;
                record.HseqCaseFileID = linkedRecord.HseqCaseFileID;
                record.HseqCaseFile = linkedRecord.HseqCaseFile;

            }
            record.CreatedBy = _RecordService.GetCurrentUser().FullName;

            record.CaseNo = linkedRecord.CaseNo;
            record.RecordNo = linkedRecord.RecordNo;

            record.LinkedRecords.Add(linkedRecord);

            if (recordType == RecordType.NCR) {
                db.NcrRecords.Add((Ncr)record);
            }
            else if (recordType == RecordType.FIS)
            {
                db.FisRecords.Add((Fis)record);
            }
            else if (recordType == RecordType.CAR)
            {
                db.CarRecords.Add((Car)record);
            }
            else if (recordType == RecordType.PAR)
            {
                db.ParRecords.Add((Par)record);
            }            

            linkedRecord.LinkedRecords.Add(record);

            db.SaveChanges();

            return record;
        }

        public void RemoveLinkedRecords(HseqRecord record)
        {
            if (record.LinkedRecords != null)
            {

                foreach (HseqRecord linkedRecord in record.LinkedRecords)
                {
                    linkedRecord.LinkedRecords.Remove(record);
                }

                record.LinkedRecords = null;
            }
        }

    }
}