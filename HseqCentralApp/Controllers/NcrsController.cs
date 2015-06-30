using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HseqCentralApp.Helpers;
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
        DelegatableService _DelegatableService;

        public NcrsController()
        {
            _RecordService = new RecordService();
            _LinkRecordService = new LinkRecordService();
            _DelegatableService = new DelegatableService();
        }


        public NcrsController(RecordService service)
        {
            _RecordService = service;
        }


        public NcrsController(LinkRecordService service)
        {
            _LinkRecordService = service;
        }

        public NcrsController(DelegatableService service)
        {
            _DelegatableService = service;
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

        // GET: Ncrs/Create
        public ActionResult Create(bool? ProposedDisposition)
        {
            NcrVM ncrVM = new NcrVM();
            Ncr ncr = new Ncr();

            ncr = (Ncr)_RecordService.PopulateRecordTypeDefaults(RecordType.NCR, ncr);
            ncr.NcrState = NcrState.New;

            //PopulateDefaults(defaults);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name");
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name");

            ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            HseqApprovalRequest hseqApprovalRequest = new HseqApprovalRequest();
            ncrVM.Ncr = ncr;
            ncrVM.HseqApprovalRequest = hseqApprovalRequest;

            //Populate defaults
            if (ProposedDisposition == null) {
                ProposedDisposition = false;
            }
            ncrVM.ProposedDisposition = (bool)ProposedDisposition;
            if (ncrVM.ProposedDisposition)
            {
                ncrVM.Ncr.NcrState = NcrState.DispositionProposed;
            }
            else if (!ncrVM.ProposedDisposition)
            {
                ncrVM.Ncr.NcrState = NcrState.New;
                ncrVM.HseqApprovalRequest.OwnerID = 1;
            }

            ncrVM.DetectedInAreas = db.BusinessAreas;
            ncrVM.ResponsibleAreas = db.BusinessAreas;
            ncrVM.DiscrepancyTypes = db.DiscrepancyTypes;
            ncrVM.Coordinators = db.HseqUsers;
            ncrVM.DispositionTypes = db.DispositionTypes;
            ncrVM.ApprovalOwners = db.HseqUsers;

            ncrVM.Ncr.RecordType = RecordType.NCR;
            ncrVM.HseqApprovalRequest.DueDate = DateTime.Now.AddDays(14);

            return View(ncrVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NcrVM ncrVM)
        {
            if (ModelState.IsValid)
            {
                Ncr ncr = ncrVM.Ncr;

                int caseNo;
                HseqCaseFile hseqCaseFile;
                ncr.CreatedBy = _RecordService.GetCurrentUser().FullName;
                ncr = (Ncr)_RecordService.CreateCaseFile(ncr, out caseNo, out hseqCaseFile, db);

                db.NcrRecords.Add(ncrVM.Ncr);
                db.SaveChanges();

                //create the folder in Alfresco and return the alfresconoderef
                //Dummy for now

                int alfresconoderef = caseNo;
                hseqCaseFile.AlfrescoNoderef = caseNo;

                ncr.AlfrescoNoderef = caseNo;

                //Create Approvals
                if (ncrVM.ProposedDisposition)
                {
                    HseqApprovalRequest hseqApprovalRequest = ncrVM.HseqApprovalRequest;
                    HseqApprovalRequest approvalRequest = _DelegatableService.AddHseqApprovalRequest(ncr, hseqApprovalRequest, db);

                    Ncr ncr2 = db.NcrRecords.Find(ncr.HseqRecordID);
                    ncr2.Delegatables.Add(approvalRequest);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            ncrVM.DetectedInAreas = db.BusinessAreas;
            ncrVM.ResponsibleAreas = db.BusinessAreas;
            ncrVM.DiscrepancyTypes = db.DiscrepancyTypes;
            ncrVM.Coordinators = db.HseqUsers;
            ncrVM.DispositionTypes = db.DispositionTypes;
            ncrVM.ApprovalOwners = db.HseqUsers;

            return View(ncrVM);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {

            Ncr ncr = (Ncr)_LinkRecordService.LinkRecord(recordId, recordSource, RecordType.NCR, db);
            //PopulateDefaults(ncr);

            TempData["recordId"] = ncr.HseqRecordID;
            TempData["recordSource"] = recordSource;

            NcrVM ncrVM = new NcrVM();
            ncrVM.Ncr = new Ncr();
            ncrVM.HseqApprovalRequest = new HseqApprovalRequest();
            ncrVM.HseqApprovalRequest.OwnerID = 1;

            ncrVM.Ncr = (Ncr)_RecordService.PopulateRecordTypeDefaults(RecordType.NCR, ncrVM.Ncr);

            ncrVM.DetectedInAreas = db.BusinessAreas;
            ncrVM.ResponsibleAreas = db.BusinessAreas;
            ncrVM.DiscrepancyTypes = db.DiscrepancyTypes;
            ncrVM.Coordinators = db.HseqUsers;
            ncrVM.DispositionTypes = db.DispositionTypes;
            ncrVM.ApprovalOwners = db.HseqUsers;

            ncrVM.Ncr.RecordType = RecordType.NCR;

            return View("Create", ncrVM);
            //return RedirectToAction("Create", new { ProposedDisposition = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,DetectedInAreaID,ResponsibleAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,CoordinatorID,ApproverID,ResponsibleParty")]Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    ncr = (Ncr)_LinkRecordService.CreateLinkRecord(ncr, recordId, recordSource, RecordType.NCR, db);

                    //Create Approvals
                    // _DelegatableService.AddHseqApprovalRequest(ncr, ncr.ApproverID, null, db);

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
            //ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.ApproverID);
            return View(ncr);
        }

        // POST: Ncrs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,DetectedInAreaID,ResponsibleAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,LinkedRecordsID,CoordinatorID,ApproverID,ResponsibleParty,CauseDesc")] Ncr ncr)
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
            //ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.ApproverID);
            return View(ncr);
        }

        public ActionResult Close(int? id)
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
            //ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.ApproverID);

            ViewBag.Status = "Close";

            return View("Edit", ncr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Close([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,DetectedInAreaID,ResponsibleAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy,LinkedRecordsID,CoordinatorID,ApproverID,ResponsibleParty,CauseDesc")] Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                ncr.LastUpdatedBy = _RecordService.GetCurrentUser().FullName;

                db.Entry(ncr).State = EntityState.Modified;

                bool errorExists = false;

                if (ncr.ResponsibleAreaID == null || ncr.ResponsibleAreaID == 0) {
                    ModelState.AddModelError("ResponsibleAreaID", "Responsible Area Cannot be empty" );
                    errorExists = true;
                }
                if (ncr.ResponsibleParty == null)
                {
                    ModelState.AddModelError("ResponsibleParty", "Responsible Party Cannot be empty");
                    errorExists = true;
                }

                var activeApprovalsCount = db.HseqApprovalRequests.Where(x => x.HseqRecordID == ncr.HseqRecordID && x.Status == ApprovalStatus.Active).Count();
                
                if (activeApprovalsCount > 0) {
                    ModelState.AddModelError("", "This record cannot be closed since there are Open Approval Requests for this record");
                    errorExists = true;
                }

                var activeTaskCount = db.HseqTasks.Where(x => x.HseqRecordID == ncr.HseqRecordID && (x.Status == TaskStatus.Active || x.Status == TaskStatus.NotStarted)).Count();
                if (activeTaskCount > 0)
                {
                    ModelState.AddModelError("", "This record cannot be closed since there are Open Tasks for this record");
                    errorExists = true;
                }


                if (!errorExists)
                {
                    ncr.NcrState = NcrState.Closed;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else {

                    ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
                    ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
                    ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
                    ViewBag.DetectedInAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.DetectedInAreaID);
                    ViewBag.ResponsibleAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.ResponsibleAreaID);
                    ViewBag.LinkedRecordsID = new SelectList(db.HseqRecords, "HseqRecordID", "LinkRecordForDisplay", ncr.LinkedRecordsID);
                    ViewBag.CoordinatorID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncr.CoordinatorID);

                    return View("Edit", ncr);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View("Edit", ncr);
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

        public ActionResult AddApproval1(int? id)
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
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            HseqApprovalRequestVM approvalRequestVM = new HseqApprovalRequestVM(ncr);
            HseqApprovalRequest hseqApprovalRequest = new HseqApprovalRequest();
            approvalRequestVM.HseqApprovalRequest = hseqApprovalRequest;
            //approvalRequestVM.Ncr = ncr;

            approvalRequestVM.HseqApprovalRequest.DateAssigned = System.DateTime.Now;
            approvalRequestVM.HseqApprovalRequest.DueDate = DateTime.Now.AddDays(14); ;
            approvalRequestVM.HseqApprovalRequest.AssigneeID = _RecordService.GetCurrentApplicationUser().HseqUserID;
            approvalRequestVM.HseqApprovalRequest.Assignee = _RecordService.GetCurrentApplicationUser(); 

            return View(approvalRequestVM);
        }

        [HttpPost]
        public ActionResult AddApproval1([Bind(Include = "Ncr, HseqApprovalRequest, ApproverID, AssigneeID")]HseqApprovalRequestVM hseqApprovalRequestVM)
        {
            Ncr ncrOrig = null;
            ncrOrig = db.NcrRecords.Find(hseqApprovalRequestVM.Ncr.HseqRecordID);
            if (ModelState.IsValid)
            {
                
                if (hseqApprovalRequestVM.ApproverID != null && hseqApprovalRequestVM.Ncr.HseqRecordID != null)
                {
                    //Abhi Create Approvals
                    // _DelegatableService.AddHseqApprovalRequest(ncrOrig, hseqApprovalRequestVM.ApproverID, hseqApprovalRequestVM.HseqApprovalRequest, db);
                    HseqApprovalRequest approvalRequest = _DelegatableService.AddHseqApprovalRequest(ncrOrig, hseqApprovalRequestVM.HseqApprovalRequest, db);
                    ncrOrig.Delegatables.Add(approvalRequest);
                    db.SaveChanges();

                    ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
                    ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return RedirectToAction("AddApproval", "Ncrs", ncrOrig.RecordNo);
            //return View(ncr);
        }

        public ActionResult AddTask1(int? id)
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
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

            HseqTaskVM taskVM = new HseqTaskVM(ncr);


            HseqTask hseqTask = new HseqTask();
            taskVM.HseqTask = hseqTask;

            taskVM.HseqTask.DateAssigned = System.DateTime.Now;
            taskVM.HseqTask.DueDate = DateTime.Now.AddDays(14); ;
            taskVM.HseqTask.AssigneeID = _RecordService.GetCurrentApplicationUser().HseqUserID;
            taskVM.HseqTask.Assignee = _RecordService.GetCurrentApplicationUser(); 

            return View(taskVM);
        }

        [HttpPost]
        public ActionResult AddTask1([Bind(Include = "Ncr, HseqTask, ApproverID")]HseqTaskVM ncrTaskVM)
        {
            Ncr ncrOrig = null;
            if (ncrTaskVM.ApproverID != null && ncrTaskVM.Ncr.HseqRecordID != null)
            {
                ncrOrig = db.NcrRecords.Find(ncrTaskVM.Ncr.HseqRecordID);

                //_DelegatableService.AddHseqTaskRequest(ncrOrig, ncrTaskVM.ApproverID, ncrTaskVM.HseqTask,  db);
                db.SaveChanges();

                ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            }

            return RedirectToAction("AddTask", "Ncrs", ncrOrig.RecordNo);
            //return View(ncr);
        }

 //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

       // GET: Ncrs/Edit/5
        public ActionResult EditApproval(int? recordId, int? approvalId)
        {
            if (recordId == null || approvalId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ncr ncr = db.NcrRecords.Find(recordId);
            HseqApprovalRequest approvalRequest = db.HseqApprovalRequests.Find(approvalId);

            if (ncr == null || approvalRequest== null)
            {
                return HttpNotFound();
            }

            NcrVM ncrVM = new NcrVM();
            ncrVM.Ncr = ncr;
            ncrVM.HseqApprovalRequest = approvalRequest;

            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqApprovalRequest.AssigneeID);
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqApprovalRequest.OwnerID);
            ncrVM.ApprovalOwners = db.HseqUsers;

            return View(ncrVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditApproval(NcrVM ncrVM)
        {

            Ncr ncr = null;
            if (ModelState.IsValid)
            {
                ncr = db.NcrRecords.Find(ncrVM.Ncr.HseqRecordID);
                ncrVM.Ncr = ncr;

                HseqApprovalRequest hseqApprovalRequest = ncrVM.HseqApprovalRequest;

                //Update Ncr Status
                if (hseqApprovalRequest.Response == ApprovalResult.Approved)
                {
                    ncr.NcrState = NcrState.DispositionApproved;
                    ncr.DateLastUpdated = DateTime.Now;
                    db.Entry(ncr).State = EntityState.Modified;
                }
                else if (hseqApprovalRequest.Response == ApprovalResult.Rejected)
                {
                    ncr.NcrState = NcrState.DispositionRejected;
                    ncr.DateLastUpdated = DateTime.Now;
                    db.Entry(ncr).State = EntityState.Modified;
                }

                db.Entry(hseqApprovalRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("OpenAction", "HseqApprovalRequests");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View(ncrVM);

        }


// //////////////// Abhi /////////////////////////////////////////////////////////////////////////////////////////
        // GET: Ncrs/Edit/5
        public ActionResult AddApproval(int? recordId)
        {
            if (recordId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ncr ncr = db.NcrRecords.Find(recordId);

            if (ncr == null)
            {
                return HttpNotFound();
            }

            HseqApprovalRequest approvalRequest = new HseqApprovalRequest();
            approvalRequest.DueDate = DateTime.Now.AddDays(14);
            approvalRequest.AssigneeID = Utils.GetCurrentApplicationUser(db).HseqUserID;

            NcrVM ncrVM = new NcrVM();
            ncrVM.Ncr = ncr;
            ncrVM.HseqApprovalRequest = approvalRequest;

            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqApprovalRequest.AssigneeID);
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqApprovalRequest.OwnerID);

            ncrVM.ApprovalOwners = db.HseqUsers;
            return View(ncrVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddApproval(NcrVM ncrVM)
        {

            Ncr ncr = null;
            if (ModelState.IsValid)
            {
                ncr = db.NcrRecords.Find(ncrVM.Ncr.HseqRecordID);
                ncrVM.Ncr = ncr;

                HseqApprovalRequest hseqApprovalRequest = ncrVM.HseqApprovalRequest;

                ncr.NcrState = NcrState.DispositionProposed;
                ncr.DateLastUpdated = DateTime.Now;
                
                HseqApprovalRequest approvalRequest = _DelegatableService.AddHseqApprovalRequest(ncr, hseqApprovalRequest, db);

                //ncr.Delegatables.Add(approvalRequest);

                db.SaveChanges();
                return RedirectToAction("OpenAction", "HseqApprovalRequests");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View(ncrVM);

        }


        public ActionResult AddTask(int? recordId)
        {
            if (recordId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ncr ncr = db.NcrRecords.Find(recordId);
            

            if (ncr == null)
            {
                return HttpNotFound();
            }

            HseqTask hseqTask = new HseqTask();
            hseqTask.DueDate = DateTime.Now.AddDays(14);

            NcrVM ncrVM = new NcrVM();
            ncrVM.Ncr = ncr;
            ncrVM.HseqTask = hseqTask;

            hseqTask.AssigneeID = Utils.GetCurrentApplicationUser(db).HseqUserID;

            ncrVM.TaskOwners = db.HseqUsers;

            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqTask.AssigneeID);
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqTask.OwnerID);
            
            return View(ncrVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTask(NcrVM ncrVM)
        {

            Ncr ncr = null;
            if (ModelState.IsValid)
            {
                ncr = db.NcrRecords.Find(ncrVM.Ncr.HseqRecordID);
                ncrVM.Ncr = ncr;

                HseqTask hseqTask = ncrVM.HseqTask;

                _DelegatableService.AddHseqTaskRequest(ncr, hseqTask, db);

                db.SaveChanges();
                return RedirectToAction("OpenAction", "HseqTasks");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View(ncrVM);
        }

// //////////////////////////////////////////

        public ActionResult EditTask(int? recordId, int? taskId)
        {
            if (recordId == null || taskId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ncr ncr = db.NcrRecords.Find(recordId);
            HseqTask task = db.HseqTasks.Find(taskId);

            if (ncr == null || task == null)
            {
                return HttpNotFound();
            }

            NcrVM ncrVM = new NcrVM();
            ncrVM.Ncr = ncr;
            ncrVM.HseqTask = task;

            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqTask.AssigneeID);
            ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqTask.OwnerID);
            ncrVM.TaskOwners = db.HseqUsers;
            return View(ncrVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask(NcrVM ncrVM)
        {

            Ncr ncr = null;
            if (ModelState.IsValid)
            {
                ncr = db.NcrRecords.Find(ncrVM.Ncr.HseqRecordID);
                ncrVM.Ncr = ncr;

                HseqTask hseqTask = ncrVM.HseqTask;

                //Update Ncr Status
                //if (hseqApprovalRequest.Response == ApprovalResult.Approved) {
                //    ncr.NcrState = NcrState.DispositionApproved;
                //    ncr.DateLastUpdated = DateTime.Now;
                //    db.Entry(ncr).State = EntityState.Modified;
                //}
                //else if (hseqApprovalRequest.Response == ApprovalResult.Rejected)
                //{
                //    ncr.NcrState = NcrState.DispositionRejected;
                //    ncr.DateLastUpdated = DateTime.Now;
                //    db.Entry(ncr).State = EntityState.Modified;
                //}

                db.Entry(hseqTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("OpenAction", "HseqTasks");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View(ncrVM);
            
        }

 

    }
}
