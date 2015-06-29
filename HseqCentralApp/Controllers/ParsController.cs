﻿using System;
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
        LinkRecordService _LinkRecordService;

        public ParsController() 
        {
            _RecordService = new RecordService();
            _LinkRecordService =    new LinkRecordService();
        }

        public ParsController(RecordService service) 
        {
            _RecordService = service;
        }

        
        public ParsController(LinkRecordService service)
        {
            _LinkRecordService = service;
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
            Par par = new Par();
            par = (Par)_RecordService.PopulateRecordTypeDefaults(RecordType.PAR, par);
            //PopulateDefaults(defaults);

           // ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(par);
        }

        // POST: Pars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Par par)
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
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(par);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {
            HseqRecord linkedRecord = _LinkRecordService.GetSourceRecord(recordId, recordSource, db);

            Par par = (Par)_LinkRecordService.LinkRecord(recordId, recordSource, RecordType.PAR, db);
            //PopulateDefaults(par);
            par = (Par)_RecordService.PopulateRecordTypeDefaults(RecordType.PAR, par);

            TempData["recordId"] = par.HseqRecordID;
            TempData["recordSource"] = recordSource;

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View("Create", par);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Par par)
        {
            if (ModelState.IsValid)
            {
                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    par = (Par)_LinkRecordService.CreateLinkRecord(par, recordId, recordSource, RecordType.PAR, db);

                    TempData["recordId"] = null;
                    TempData["recordSource"] = null;
                }

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

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
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", par.CoordinatorID);

            return View(par);
        }

        // POST: Pars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Par par)
        {
            if (ModelState.IsValid)
            {
                par.LastUpdatedBy = _RecordService.GetCurrentUser().FullName;

                db.Entry(par).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", par.CoordinatorID);

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

            _LinkRecordService.RemoveLinkedRecords(par);

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
            //ViewBag.QualityCoordinator = defaults.QualityCoordinator;
            //ViewBag.NcrState = defaults.NcrState;
        }
    }
}
