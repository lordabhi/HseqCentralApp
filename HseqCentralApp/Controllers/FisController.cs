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

            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");

            return View();
        }


        // POST: Fis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,MainRecordId,HseqCaseFileID,Category,NcrSource,FisCodeID,BusinessAreaID,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Fis fis)
        {
            if (ModelState.IsValid)
            {
                int caseNo;
                HseqCaseFile hseqCaseFile;
                fis.CreatedBy = _RecordService.GetCurrentUser().FullName;
                fis = (Fis)_RecordService.CreateCaseFile(fis, out caseNo, out hseqCaseFile, db);

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

            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1", fis.FisCodeID);
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            return View(fis);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {
            HseqRecord linkedRecord = _RecordService.GetSourceRecord(recordId, recordSource, db);

            var defaults = _RecordService.PopulateRecordTypeLinked(linkedRecord, RecordType.FIS);
            PopulateDefaults(defaults);

            Fis fis = new Fis(linkedRecord);
            fis.RecordType = RecordType.FIS;
            fis.HseqRecordID = linkedRecord.HseqRecordID;

            TempData["recordId"] = linkedRecord.HseqRecordID;
            TempData["recordSource"] = recordSource;

            linkedRecord.LinkedRecords.Add(fis);

            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);

            return View("Create", fis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,MainRecordId,HseqCaseFileID,Category,NcrSource,FisCodeID,BusinessAreaID,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Fis fis)
        {
            if (ModelState.IsValid)
            {
                fis.CreatedBy = _RecordService.GetCurrentUser().FullName;
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

            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1", fis.FisCodeID);
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

            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            return View(fis);
        }

        // POST: Fis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,MainRecordId,HseqCaseFileID,Category,NcrSource,FisCodeID,BusinessAreaID,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Fis fis)
        {
            if (ModelState.IsValid)
            {
                fis.CreatedBy = _RecordService.GetCurrentUser().FullName;
                db.Entry(fis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1", fis.FisCodeID);
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

            Fis fis = db.FisRecords.Find(id);

            _RecordService.RemoveLinkedRecords(fis);

            //if (fis.LinkedRecords != null)
            //{

            //    foreach (HseqRecord linkedRecord in fis.LinkedRecords)
            //    {
            //        linkedRecord.LinkedRecords.Remove(fis);
            //    }

            //    fis.LinkedRecords = null;
            //}

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

        private void PopulateDefaults(dynamic defaults)
        {
            ViewBag.RecordType = defaults.RecordType;
            ViewBag.EnteredBy = defaults.EnteredBy;
            ViewBag.ReportedBy = defaults.ReportedBy;
            ViewBag.QualityCoordinator = defaults.QualityCoordinator;
            ViewBag.NcrState = defaults.NcrState;
        }

        private System.Collections.IEnumerable getCodeCategoryList()
        {
            //SELECT ' -> ' + Districts.DistrictName, Districts.Id, Districts.StateId FROM Districts UNION SELECT States.StateName, -1 , States.Id FROM States ORDER BY Districts.StateId,Districts.Id

            return (
                        from FisCodes in db.FisCodes
                        select new
                        {
                            Column1 = (" -> " + FisCodes.CodeName),
                            Id = FisCodes.Id,
                            FisCategoryId = FisCodes.FisCategoryId
                        }
                    ).Union
                    (
                        from FisCategories in db.FisCategories
                        select new
                        {
                            Column1 = FisCategories.Name,
                            Id = (-1),
                            FisCategoryId = FisCategories.Id
                        }
                    ).OrderBy(p => p.FisCategoryId).ThenBy(p => p.Id).ToList();
        }
    }
}
