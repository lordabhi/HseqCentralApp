using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HseqCentralApp.Models;
using HseqCentralApp.Services;

namespace HseqCentralApp.Controllers
{
    public class FisController : Controller
    {
        //private HseqCentralAppContext db = new HseqCentralAppContext();
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;

        public FisController() : this(new RecordService()){}

        public FisController(RecordService service) 
        {
            _RecordService = service;
        }

        // GET: Fis
        public ActionResult Index()
        {
            var hseqRecords = db.FisRecords.Include(f => f.HseqCaseFile);
            return View(hseqRecords.ToList());
        }

        // GET: Fis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fis fis = db.FisRecords.Find(id);
            if (fis == null)
            {
                return HttpNotFound();
            }
            return View(fis);
        }

        // GET: Fis/Create
        public ActionResult Create()
        {

            var defaults = _RecordService.PopulateRecordTypeDefaults(RecordType.FIS);

            PopulateDefaults(defaults);

            //ViewBag.RecordType = RecordType.FIS;
            //ViewBag.EnteredBy = "Test User";
            //ViewBag.ReportedBy = "Test User, Sales";
            //ViewBag.QualityCoordinator = "Mr. Paul Smith";
            //ViewBag.Status = "Pending";

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");

            return View();
        }

        private void PopulateDefaults(dynamic defaults)
        {
            ViewBag.RecordType = defaults.RecordType;
            ViewBag.EnteredBy = defaults.EnteredBy;
            ViewBag.ReportedBy = defaults.ReportedBy;
            ViewBag.QualityCoordinator = defaults.QualityCoordinator;
            //ViewBag.Status = defaults.Status;
            ViewBag.NcrState = defaults.NcrState;
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {
            HseqRecord linkedRecord = _RecordService.GetSourceRecord(recordId, recordSource, db);

            //HseqRecord linkedRecord = db.NcrRecords.Find(recordId);
            
            Fis fis = new Fis(linkedRecord);
            fis.RecordType = RecordType.FIS;
            fis.HseqRecordID = linkedRecord.HseqRecordID;

            ViewBag.EnteredBy = linkedRecord.EnteredBy;
            ViewBag.ReportedBy = linkedRecord.ReportedBy;
            ViewBag.QualityCoordinator = linkedRecord.QualityCoordinator;
            //ViewBag.Status = "Pending";

            TempData["recordId"] = linkedRecord.HseqRecordID;
            TempData["recordSource"] = recordSource;

            linkedRecord.LinkedRecords.Add(fis);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name");
            return View("Create", fis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked(Fis fis)
        {
            if (ModelState.IsValid)
            {

                db.FisRecords.Add(fis);
                db.SaveChanges();

                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    //HseqRecord linkedRecord = db.NcrRecords.Find(recordId);
                    HseqRecord linkedRecord = _RecordService.GetSourceRecord(recordId, recordSource, db);

                    if (linkedRecord != null)
                    {
                        fis.AlfrescoNoderef = linkedRecord.AlfrescoNoderef;
                        fis.HseqCaseFileID = linkedRecord.HseqCaseFileID;
                        fis.HseqCaseFile = linkedRecord.HseqCaseFile;

                    }

                    fis.LinkedRecords.Add(linkedRecord);
                    linkedRecord.LinkedRecords.Add(fis);

                    TempData["recordId"] = null;
                    TempData["recordSource"] = null;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            //ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", fis.DiscrepancyTypeID);
            return View(fis);
        }


        // POST: Fis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,MainRecordId,HseqCaseFileID,Category,NcrSource")] Fis fis)
        {
            if (ModelState.IsValid)
            {

                int caseNo = _RecordService.GetNextCaseNumber(db);

                HseqCaseFile hseqCaseFile = new HseqCaseFile();

                hseqCaseFile.CaseNo = caseNo;

                db.HseqCaseFiles.Add(hseqCaseFile);

                fis.HseqCaseFile = hseqCaseFile;
                fis.HseqCaseFileID = hseqCaseFile.HseqCaseFileID;

                /////
                hseqCaseFile.HseqRecords.Add(fis);

                db.FisRecords.Add(fis);
                db.SaveChanges();

                //create the folder in Alfresco and return the alfresconoderef
                //Dummy for now

                int alfresconoderef = caseNo;
                hseqCaseFile.AlfrescoNoderef = caseNo;

                fis.AlfrescoNoderef = caseNo;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            return View(fis);
        }

        // GET: Fis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fis fis = db.FisRecords.Find(id);
            if (fis == null)
            {
                return HttpNotFound();
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            return View(fis);
        }

        // POST: Fis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,MainRecordId,HseqCaseFileID,Category,NcrSource")] Fis fis)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            return View(fis);
        }

        // GET: Fis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fis fis = db.FisRecords.Find(id);
            if (fis == null)
            {
                return HttpNotFound();
            }
            return View(fis);
        }

        // POST: Fis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Fis fis = db.FisRecords.Find(id);
            //db.FisRecords.Remove(fis);
            //db.SaveChanges();
            //return RedirectToAction("Index");

            Fis fis = db.FisRecords.Find(id);

            if (fis.LinkedRecords != null)
            {

                foreach (HseqRecord linkedRecord in fis.LinkedRecords)
                {
                    linkedRecord.LinkedRecords.Remove(fis);
                }

                fis.LinkedRecords = null;
            }

            int? caseFileId = fis.HseqCaseFileID;

            db.FisRecords.Remove(fis);

            //Remove the Case folder also if this is the last record
            var caseFileRefCounts = db.HseqRecords.Where(x => x.HseqCaseFileID == caseFileId).Count();

            //Remove the case file if this is the last node
            if (caseFileRefCounts == 1)
            {
                HseqCaseFile hc = db.HseqCaseFiles.Find(caseFileId);
                db.HseqCaseFiles.Remove(hc);

            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
