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
using HseqCentralApp.ViewModels;

namespace HseqCentralApp.Controllers
{
    //[Authorize(Roles = "Admin, CanEdit")]
    public class NcrsController : Controller
    {

        //private HseqCentralAppContext db = new HseqCentralAppContext();
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;
        LinkRecordService _LinkRecordService;
        ApprovalService _ApprovalService;

        public NcrsController()
        {
            _RecordService = new RecordService();
            _LinkRecordService = new LinkRecordService();
            _ApprovalService = new ApprovalService();
        }


        public NcrsController(RecordService service)
        {
            _RecordService = service;
        }


        public NcrsController(LinkRecordService service)
        {
            _LinkRecordService = service;
        }

        public NcrsController(ApprovalService service)
        {
            _ApprovalService = service;
        }

        // GET: Ncrs
        public ActionResult Index()
        {
            var hseqRecords = db.NcrRecords.Include(n => n.HseqCaseFile).Include(n => n.DiscrepancyType);
            return View(hseqRecords.ToList());
        }

        // GET: Ncrs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ncr ncr = db.NcrRecords.Find(id);
            if (ncr == null)
            {
                return HttpNotFound();
            }
            return View(ncr);
        }


        //public ActionResult PendingApproval()
        //{
        //    var currentUser = _RecordService.GetCurrentUser();

        //    var x = from a in db.ApproverDispositions
        //            where a.ApproverID == currentUser.Id
        //            select a;

        //    var xid = x.FirstOrDefault().ApproverDispositionID;
        //    IEnumerable<Ncr> PendingApprovals = from ncr in db.NcrRecords
        //                                        where ncr.DispositionApproverID == xid.ToString()
        //                                        select ncr;
        //    ViewBag.PendingApprovals = PendingApprovals;
        //    return View("PendingApprovals");
        //}

        // GET: Ncrs/Create
        public ActionResult Create()
        {
            var defaults = _RecordService.PopulateRecordTypeDefaults(RecordType.NCR);

            PopulateDefaults(defaults);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name");
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name");
            //ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");

            ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View();
        }


        // POST: Ncrs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,DetectedInAreaID,ResponsibleAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID,ApproverID")] Ncr ncr)
        {

            if (ModelState.IsValid)
            {

                int caseNo;
                HseqCaseFile hseqCaseFile;
                ncr.CreatedBy = _RecordService.GetCurrentUser().FullName;
                ncr = (Ncr)_RecordService.CreateCaseFile(ncr, out caseNo, out hseqCaseFile, db);

                db.NcrRecords.Add(ncr);
                db.SaveChanges();

                //create the folder in Alfresco and return the alfresconoderef
                //Dummy for now

                int alfresconoderef = caseNo;
                hseqCaseFile.AlfrescoNoderef = caseNo;

                ncr.AlfrescoNoderef = caseNo;

                //Abhi Create Approvals
                _ApprovalService.AddHseqApprovalRequest(ncr, ncr.ApproverID, db);
                
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            //ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");

            ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.DetectedInAreaID);
            ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.ResponsibleAreaID);

            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(ncr);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {

            Ncr ncr = (Ncr)_LinkRecordService.LinkRecord(recordId, recordSource, RecordType.NCR, db);
            PopulateDefaults(ncr);

            TempData["recordId"] = ncr.HseqRecordID;
            TempData["recordSource"] = recordSource;


            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            //ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");
            ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.DetectedInAreaID);
            ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.ResponsibleAreaID);

            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View("Create", ncr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,DetectedInAreaID,ResponsibleAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID,ApproverID")]Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    ncr = (Ncr)_LinkRecordService.CreateLinkRecord(ncr, recordId, recordSource, RecordType.NCR, db);

                    //Create Approvals
                    _ApprovalService.AddHseqApprovalRequest(ncr, ncr.ApproverID, db);

                    TempData["recordId"] = null;
                    TempData["recordSource"] = null;
                }

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            //ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");
            ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.DetectedInAreaID);
            ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.ResponsibleAreaID);

            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(ncr);
        }



        // GET: Ncrs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ncr ncr = db.NcrRecords.Find(id);
            if (ncr == null)
            {
                return HttpNotFound();
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            //ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName", ncr.DispositionApproverID);
            ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.DetectedInAreaID);
            ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.ResponsibleAreaID);
            ViewBag.LinkedRecordsID = new SelectList(db.HseqRecords, "HseqRecordID", "LinkRecordForDisplay");

            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.CoordinatorID);
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.ApproverID);
            return View(ncr);
        }

        // POST: Ncrs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,DetectedInAreaID,ResponsibleAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,LinkedRecordsID,CoordinatorID,ApproverID")] Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                ncr.LastUpdatedBy = _RecordService.GetCurrentUser().FullName;

                db.Entry(ncr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            //ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName", ncr.DispositionApproverID);
            ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.DetectedInAreaID);
            ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.ResponsibleAreaID);
            ViewBag.LinkedRecordsID = new SelectList(db.HseqRecords, "HseqRecordID", "LinkRecordForDisplay", ncr.LinkedRecordsID);

            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.CoordinatorID);
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.ApproverID);
            return View(ncr);
        }

        // GET: Ncrs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ncr ncr = db.NcrRecords.Find(id);
            if (ncr == null)
            {
                return HttpNotFound();
            }
            return View(ncr);
        }

        // POST: Ncrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ncr ncr = db.NcrRecords.Find(id);

            _LinkRecordService.RemoveLinkedRecords(ncr);

            //if (ncr.LinkedRecords != null)
            //{

            //    foreach (HseqRecord linkedRecord in ncr.LinkedRecords)
            //    {
            //        linkedRecord.LinkedRecords.Remove(ncr);
            //    }

            //    ncr.LinkedRecords = null;
            //}

            int? caseFileId = ncr.HseqCaseFileID;

            db.NcrRecords.Remove(ncr);

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
            //ViewBag.Status = defaults.Status;
            ViewBag.NcrState = defaults.NcrState;
        }

        public ActionResult LinkExistingRecord(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ncr ncr = db.NcrRecords.Find(id);
            if (ncr == null)
            {
                return HttpNotFound();
            }

            //TODO: Filter out the already linked records 
            var availableRecords = from h in db.HseqRecords
                                   where h.RecordType != RecordType.NCR
                                   select h;

            ViewBag.LinkedRecordsID = new SelectList(availableRecords, "HseqRecordID", "LinkRecordForDisplay");

            return View(ncr);
        }


        [HttpPost]
        public ActionResult LinkExistingRecord(Ncr ncr, int? LinkedRecordsID)
        {
            if (LinkedRecordsID != null)
            {
                Ncr ncrOrig = db.NcrRecords.Find(ncr.HseqRecordID);
                HseqRecord LinkedRecord = db.HseqRecords.Find(LinkedRecordsID);

                if (ncrOrig != null && LinkedRecord != null)
                {
                    ncrOrig.LinkedRecords.Add(LinkedRecord);
                    LinkedRecord.LinkedRecords.Add(ncrOrig);

                    db.SaveChanges();
                }
                var availableRecords = from h in db.HseqRecords
                                       where h.RecordType != RecordType.NCR
                                       select h;

                ViewBag.LinkedRecordsID = new SelectList(availableRecords, "HseqRecordID", "LinkRecordForDisplay");
            }

            return RedirectToAction("LinkExistingRecord", "Ncrs", ncr.RecordNo);
            //return View(ncr);
        }

        //Abhi
        public ActionResult AddApproval(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ncr ncr = db.NcrRecords.Find(id);
            if (ncr == null)
            {
                return HttpNotFound();
            }

            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            return View(ncr);
        }

        [HttpPost]
        public ActionResult AddApproval(Ncr ncr, int? ApproverID)
        {
            if (ApproverID != null)
            {
                Ncr ncrOrig = db.NcrRecords.Find(ncr.HseqRecordID);

                //Abhi Create Approvals
                _ApprovalService.AddHseqApprovalRequest(ncrOrig, ApproverID, db);
                db.SaveChanges();

                ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            }

            return RedirectToAction("AddApproval", "Ncrs", ncr.RecordNo);
            //return View(ncr);
        }

        public ActionResult AddTask(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ncr ncr = db.NcrRecords.Find(id);
            if (ncr == null)
            {
                return HttpNotFound();
            }

            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            NcrTaskVM TaskVM = new NcrTaskVM(ncr);

            return View(TaskVM);
        }

        [HttpPost]
        public ActionResult AddTask([Bind(Include = "Ncr, HseqTask, ApproverID")]NcrTaskVM ncrTaskVM)
        {
            Ncr ncrOrig = null;
            if (ncrTaskVM.ApproverID != null && ncrTaskVM.Ncr.HseqRecordID != null)
            {
                ncrOrig = db.NcrRecords.Find(ncrTaskVM.Ncr.HseqRecordID);

                _ApprovalService.AddHseqTaskRequest(ncrOrig, ncrTaskVM.ApproverID, ncrTaskVM.HseqTask,  db);
                db.SaveChanges();

                ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            }

            return RedirectToAction("AddTask", "Ncrs", ncrOrig.RecordNo);
            //return View(ncr);
        }


    }
}
