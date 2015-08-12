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
using DevExpress.Web.Mvc;
using HseqCentralApp.Helpers;
using HseqCentralApp.ViewModels;

namespace HseqCentralApp.Controllers
{
    public class FisController : Controller
    {
        //private HseqCentralAppContext db = new HseqCentralAppContext();
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;
        LinkRecordService _LinkRecordService;

        //public FisController() : this(new RecordService()){}

        public FisController() 
        {
            _RecordService = new RecordService();
            _LinkRecordService =    new LinkRecordService();
        }

        public FisController(LinkRecordService service)
        {
            _LinkRecordService = service;
        }

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
            Fis fis = new Fis();

            fis = (Fis)_RecordService.PopulateRecordTypeDefaults(RecordType.FIS, fis);

            //PopulateDefaults(defaults);

            //ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(fis);
        }


        // POST: Fis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,MainRecordId,HseqCaseFileID,Category,FisSource,FisCodeID,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Fis fis)
        {
            if (ModelState.IsValid)
            {
                string caseNo;
                HseqCaseFile hseqCaseFile;
                fis.CreatedBy = _RecordService.GetCurrentUser().FullName;
                fis = (Fis)_RecordService.CreateCaseFile(fis, out caseNo, out hseqCaseFile, db);

                db.FisRecords.Add(fis);
                db.SaveChanges();

                //create the folder in Alfresco and return the alfresconoderef
                //Dummy for now

                //int alfresconoderef = caseNo;
                //hseqCaseFile.AlfrescoNoderef = caseNo;

                //fis.AlfrescoNoderef = caseNo;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1", fis.FisCodeID);
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(fis);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {
            Fis fis = (Fis)_LinkRecordService.CreateLinkedRecord(recordId, recordSource, RecordType.FIS, db);
            //PopulateDefaults(fis);
            fis = (Fis)_RecordService.PopulateRecordTypeDefaults(RecordType.FIS, fis);

            TempData["recordId"] = fis.HseqRecordID;
            TempData["recordSource"] = recordSource;

            //ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View("Create", fis);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,MainRecordId,HseqCaseFileID,Category,FisSource,FisCodeID,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Fis fis)
        {
            if (ModelState.IsValid)
            {
                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    fis = (Fis)_LinkRecordService.CreateLinkRecord(fis, recordId, recordSource, RecordType.FIS, db);

                    TempData["recordId"] = null;
                    TempData["recordSource"] = null;
                }

                return RedirectToAction("Index");
            }

            //ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1", fis.FisCodeID);
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

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

            //ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", fis.CoordinatorID);

            return View(fis);
        }

        // POST: Fis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,MainRecordId,HseqCaseFileID,Category,FisSource,FisCodeID,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID")] Fis fis)
        {
            if (ModelState.IsValid)
            {
                fis.CreatedBy = _RecordService.GetCurrentUser().FullName;
                db.Entry(fis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", fis.BusinessAreaID);
            ViewBag.CodeCategoryList = new SelectList(getCodeCategoryList(), "Id", "Column1", fis.FisCodeID);
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", fis.HseqCaseFileID);
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", fis.CoordinatorID);

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

            _LinkRecordService.RemoveLinkedRecords(fis);

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
            //ViewBag.QualityCoordinator = defaults.QualityCoordinator;
            //ViewBag.NcrState = defaults.NcrState;
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


        HseqCentralApp.Models.ApplicationDbContext db1 = new HseqCentralApp.Models.ApplicationDbContext();

        [ValidateInput(false)]
        public ActionResult FisGridViewPartial()
        {
            //var model = db1.FisRecords;
            var model = NavigationUtils.GetFilteredFisRecords();

            List<Fis> filteredFisRecords = NavigationUtils.GetFilteredFisRecords();
            if (filteredFisRecords != null)
            {
                NavigationFilter.FilteredFisRecordIds = filteredFisRecords.Select(record => record.HseqRecordID).OrderBy(HseqRecordID => HseqRecordID).ToList();
            }
            else
            {
                NavigationFilter.FilteredFisRecordIds = new List<int>();
            }

            return PartialView("_FisGridView", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FisGridViewPartialAddNew(Fis item)
        {
            var model = db1.FisRecords;
            if (ModelState.IsValid)
            {
                try
                {
                    model.Add(item);
                    db1.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_FisGridView", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FisGridViewPartialUpdate(Fis item)
        {
            var model = db1.FisRecords;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(it => it.HseqRecordID == item.HseqRecordID);
                    if (modelItem != null)
                    {
                        this.UpdateModel(modelItem);
                        db1.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_FisGridView", model.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FisGridViewPartialDelete(System.Int32 HseqRecordID)
        {
            var model = db1.FisRecords;
            if (HseqRecordID >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.HseqRecordID == HseqRecordID);
                    if (item != null)
                        model.Remove(item);
                    db1.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_FisGridView", model.ToList());
        }

        HseqCentralApp.Models.ApplicationDbContext db2 = new HseqCentralApp.Models.ApplicationDbContext();

        public ActionResult _FisChartContainer()
        {
            //var model = db2.FisRecords;
            var model = NavigationUtils.GetFilteredFisRecords();
            return PartialView("_FisChartContainer", model.ToList());
        }

        //public ActionResult FisGridViewPanel()
        //{
        //    //return PartialView("~/Views/Shared/_NcrChartContainer.cshtml", model);
        //    return PartialView("_FisGridViewPanel");
        //}

    }
}
