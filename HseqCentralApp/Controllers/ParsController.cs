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
    public class ParsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;

        public ParsController() : this(new RecordService()){}

        public ParsController(RecordService service) 
        {
            _RecordService = service;
        }

        // GET: Pars
        public ActionResult Index()
        {
            var hseqRecords = db.ParRecords.Include(p => p.HseqCaseFile);
            return View(hseqRecords.ToList());
        }

        // GET: Pars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Par par = db.ParRecords.Find(id);
            if (par == null)
            {
                return HttpNotFound();
            }
            return View(par);
        }

        // GET: Pars/Create
        public ActionResult Create()
        {
            var defaults = _RecordService.PopulateRecordTypeDefaults(RecordType.PAR);
            PopulateDefaults(defaults);

            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            return View();
        }

        // POST: Pars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Par par)
        {
            if (ModelState.IsValid)
            {
                int caseNo;
                HseqCaseFile hseqCaseFile;
                par.CreatedBy = _RecordService.GetCurrentUser().FullName;
                par = (Par)_RecordService.CreateCaseFile(par, out caseNo, out hseqCaseFile, db);

                db.HseqRecords.Add(par);
                db.SaveChanges();

                //create the folder in Alfresco and return the alfresconoderef
                //Dummy for now

                int alfresconoderef = caseNo;
                hseqCaseFile.AlfrescoNoderef = caseNo;

                par.AlfrescoNoderef = caseNo;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);
            return View(par);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {
            HseqRecord linkedRecord = _RecordService.GetSourceRecord(recordId, recordSource, db);

            var defaults = _RecordService.PopulateRecordTypeLinked(linkedRecord, RecordType.PAR);
            PopulateDefaults(defaults);

            Par par = new Par(linkedRecord);
            par.RecordType = RecordType.PAR;
            par.HseqRecordID = linkedRecord.HseqRecordID;

            TempData["recordId"] = linkedRecord.HseqRecordID;
            TempData["recordSource"] = recordSource;

            linkedRecord.LinkedRecords.Add(par);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);

            return View("Create", par);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Par par)
        {
            if (ModelState.IsValid)
            {
                par.CreatedBy = _RecordService.GetCurrentUser().FullName;

                db.ParRecords.Add(par);
                //db.SaveChanges();

                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    HseqRecord linkedRecord = _RecordService.GetSourceRecord(recordId, recordSource, db);

                    if (linkedRecord != null)
                    {
                        par.AlfrescoNoderef = linkedRecord.AlfrescoNoderef;
                        par.HseqCaseFileID = linkedRecord.HseqCaseFileID;
                        par.HseqCaseFile = linkedRecord.HseqCaseFile;

                    }

                    par.CaseNo = linkedRecord.CaseNo;
                    par.RecordNo = linkedRecord.RecordNo;

                    par.LinkedRecords.Add(linkedRecord);
                    linkedRecord.LinkedRecords.Add(par);

                    TempData["recordId"] = null;
                    TempData["recordSource"] = null;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);

            return View(par);
        }

        // GET: Pars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Par par = db.ParRecords.Find(id);
            if (par == null)
            {
                return HttpNotFound();
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);
            return View(par);
        }

        // POST: Pars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Par par)
        {
            if (ModelState.IsValid)
            {
                par.LastUpdatedBy = _RecordService.GetCurrentUser().FullName;

                db.Entry(par).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);
            return View(par);
        }

        // GET: Pars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Par par = db.ParRecords.Find(id);
            if (par == null)
            {
                return HttpNotFound();
            }
            return View(par);
        }

        // POST: Pars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Par par = db.ParRecords.Find(id);

            _RecordService.RemoveLinkedRecords(par);

            int? caseFileId = par.HseqCaseFileID;

            db.HseqRecords.Remove(par);

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

        private void PopulateDefaults(dynamic defaults)
        {
            ViewBag.RecordType = defaults.RecordType;
            ViewBag.EnteredBy = defaults.EnteredBy;
            ViewBag.ReportedBy = defaults.ReportedBy;
            ViewBag.QualityCoordinator = defaults.QualityCoordinator;
            ViewBag.NcrState = defaults.NcrState;
        }
    }
}
