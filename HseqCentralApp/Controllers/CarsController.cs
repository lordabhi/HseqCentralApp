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
    public class CarsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;
        LinkRecordService _LinkRecordService;

        public CarsController() 
        {
            _RecordService = new RecordService();
            _LinkRecordService =    new LinkRecordService();
        }

        public CarsController(RecordService service) 
        {
            _RecordService = service;
        }
        
        public CarsController(LinkRecordService service)
        {
            _LinkRecordService = service;
        }
        // GET: Cars
        public ActionResult Index()
        {
            var hseqRecords = db.CarRecords.Include(f => f.HseqCaseFile);
            return View(hseqRecords.ToList());

        }

        // GET: Cars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.CarRecords.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // GET: Cars/Create
        public ActionResult Create()
        {
            var defaults = _RecordService.PopulateRecordTypeDefaults(RecordType.CAR);
            PopulateDefaults(defaults);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Car car)
        {
            if (ModelState.IsValid)
            {
                int caseNo;
                HseqCaseFile hseqCaseFile;
                car.CreatedBy = _RecordService.GetCurrentUser().FullName;
                car = (Car)_RecordService.CreateCaseFile(car, out caseNo, out hseqCaseFile, db);

                db.HseqRecords.Add(car);
                db.SaveChanges();

                //create the folder in Alfresco and return the alfresconoderef
                //Dummy for now

                int alfresconoderef = caseNo;
                hseqCaseFile.AlfrescoNoderef = caseNo;

                car.AlfrescoNoderef = caseNo;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", car.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            return View(car);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {
            Car car = (Car)_LinkRecordService.LinkRecord(recordId, recordSource, RecordType.CAR, db);
            PopulateDefaults(car);

            TempData["recordId"] = car.HseqRecordID;
            TempData["recordSource"] = recordSource;

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", car.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View("Create", car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Car car)
        {
            if (ModelState.IsValid)
            {

                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    car = (Car)_LinkRecordService.CreateLinkRecord(car, recordId, recordSource, RecordType.CAR, db);

                    TempData["recordId"] = null;
                    TempData["recordSource"] = null;
                }

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", car.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(car);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.CarRecords.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", car.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", car.CoordinatorID);

            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,status,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Car car)
        {
            if (ModelState.IsValid)
            {
                car.LastUpdatedBy = _RecordService.GetCurrentUser().FullName;

                db.Entry(car).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", car.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", car.CoordinatorID);

            return View(car);
        }

        // GET: Cars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.CarRecords.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Car car = db.CarRecords.Find(id);

            _LinkRecordService.RemoveLinkedRecords(car);

            int? caseFileId = car.HseqCaseFileID;

            db.HseqRecords.Remove(car);

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
