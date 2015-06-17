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
    //[Authorize(Roles = "Admin, CanEdit")]
    public class NcrsController : Controller
    {

        //private HseqCentralAppContext db = new HseqCentralAppContext();
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;

        public NcrsController() : this(new RecordService()){}

        public NcrsController(RecordService service) 
        {
            _RecordService = service;
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

        
            public ActionResult PendingApproval(){

                //var remainingUsers = from u in db.Users
                //                     where !(from a in db.ApproverDispositions
                //                             where a.ApproverID == u.Id
                //                             select a.ApproverID).Contains(u.Id)
                //                     select u;

                var currentUser = _RecordService.GetCurrentUser();

                var x = from a in db.ApproverDispositions
                        where a.ApproverID == currentUser.Id
                        select a;

                var xid = x.FirstOrDefault().ApproverDispositionID;
                IEnumerable<Ncr> PendingApprovals = from ncr in db.NcrRecords
                                                    where ncr.DispositionApproverID == xid.ToString()
                                                    select ncr;

                ViewBag.PendingApprovals = PendingApprovals;

                
                return View("PendingApprovals");

            }

        // GET: Ncrs/Create
        public ActionResult Create()
        {
          var defaults = _RecordService.PopulateRecordTypeDefaults(RecordType.NCR);

           PopulateDefaults(defaults);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name");
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name");
            ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");

            return View();
        }


        // POST: Ncrs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,BusinessAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Ncr ncr)
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
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.BusinessAreaID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");

            return View(ncr);
        }

        public ActionResult CreateLinked(int recordId, String recordSource)
        {
            HseqRecord linkedRecord = _RecordService.GetSourceRecord(recordId, recordSource, db);

            var defaults =_RecordService.PopulateRecordTypeLinked(linkedRecord, RecordType.NCR);
            PopulateDefaults(defaults);

            Ncr ncr = new Ncr(linkedRecord);
            ncr.RecordType = RecordType.NCR;
            ncr.HseqRecordID = linkedRecord.HseqRecordID;

            TempData["recordId"] = linkedRecord.HseqRecordID;
            TempData["recordSource"] = recordSource;

            linkedRecord.LinkedRecords.Add(ncr);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.BusinessAreaID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");

            return View("Create", ncr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,BusinessAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")]Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                ncr.CreatedBy = _RecordService.GetCurrentUser().FullName;

                db.NcrRecords.Add(ncr);
                db.SaveChanges();

                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];
                    var recordSource = (string)TempData["recordSource"];

                    HseqRecord linkedRecord = _RecordService.GetSourceRecord(recordId, recordSource, db);

                    if (linkedRecord != null)
                    {
                        ncr.AlfrescoNoderef = linkedRecord.AlfrescoNoderef;
                        ncr.HseqCaseFileID = linkedRecord.HseqCaseFileID;
                        ncr.HseqCaseFile = linkedRecord.HseqCaseFile;

                    }

                    ncr.LinkedRecords.Add(linkedRecord);
                    linkedRecord.LinkedRecords.Add(ncr);

                    TempData["recordId"] = null;
                    TempData["recordSource"] = null;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.BusinessAreaID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName");

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
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.BusinessAreaID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName", ncr.DispositionApproverID);

            return View(ncr);
        }

        // POST: Ncrs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,NcrSource,NcrState,DiscrepancyTypeID,BusinessAreaID,DispositionTypeID,DispositionApproverID,DispositionNote,DateCreated,DateLastUpdated,CreatedBy,LastUpdatedBy")] Ncr ncr)            
        {
            if (ModelState.IsValid)
            {
                ncr.LastUpdatedBy = _RecordService.GetCurrentUser().FullName;

                db.Entry(ncr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }else {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", ncr.BusinessAreaID);
            ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            ViewBag.DispositionApproverID = new SelectList(db.ApproverDispositions, "ApproverDispositionID", "FullName", ncr.DispositionApproverID);

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

            _RecordService.RemoveLinkedRecords(ncr);
            
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
            ViewBag.QualityCoordinator = defaults.QualityCoordinator;
            //ViewBag.Status = defaults.Status;
            ViewBag.NcrState = defaults.NcrState;
        }

    }

}
