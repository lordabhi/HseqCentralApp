using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HseqCentralApp.Models;

namespace HseqCentralApp.Controllers
{
    public class NcrsController : Controller
    {
        private HseqCentralAppContext db = new HseqCentralAppContext();

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
        public ActionResult Create()
        {
            ViewBag.RecordType = RecordType.NCR;
            ViewBag.EnteredBy = "Test User";
            ViewBag.ReportedBy = "Test User, Sales";
            ViewBag.QualityCoordinator = "Mr. Paul Smith";
            ViewBag.Status = "Pending";

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name");
            return View();
        }


        public ActionResult CreateLinked(int recordId)
        {
            HseqRecord linkedRecord = db.FisRecords.Find(recordId);
            Ncr ncr = new Ncr(linkedRecord);
            ncr.RecordType = RecordType.NCR;
            ncr.HseqRecordID = linkedRecord.HseqRecordID;

            ViewBag.EnteredBy = linkedRecord.EnteredBy;
            ViewBag.ReportedBy = linkedRecord.ReportedBy;
            ViewBag.QualityCoordinator = linkedRecord.QualityCoordinator;
            //ViewBag.Status = "Pending";

            TempData["recordId"] = linkedRecord.HseqRecordID;

            linkedRecord.LinkedRecords.Add(ncr);

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name");
            return View("Create", ncr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLinked(Ncr ncr)
        {
            if (ModelState.IsValid)
            {

                db.NcrRecords.Add(ncr);
                db.SaveChanges();

                if (TempData["recordId"] != null)
                {
                    var recordId = (int)TempData["recordId"];

                    HseqRecord linkedRecord = db.FisRecords.Find(recordId);

                    if (linkedRecord != null)
                    {
                        ncr.AlfrescoNoderef = linkedRecord.AlfrescoNoderef;
                        ncr.HseqCaseFileID = linkedRecord.HseqCaseFileID;
                        ncr.HseqCaseFile = linkedRecord.HseqCaseFile;

                    }

                    ncr.LinkedRecords.Add(linkedRecord);
                    linkedRecord.LinkedRecords.Add(ncr);

                    TempData["recordId"] = null;
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
            return View(ncr);
        }

        // POST: Ncrs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,NcrSource,NcrState,DiscrepancyTypeID")] Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                int caseNo = 1;

                IList<HseqCaseFile> hseqCaseFilesList = db.HseqCaseFiles.ToList();

                if (hseqCaseFilesList != null && hseqCaseFilesList.LongCount() > 0)
                {

                    caseNo = hseqCaseFilesList.Max(p => p.CaseNo) + 1;

                }

                HseqCaseFile hseqCaseFile = new HseqCaseFile();

                hseqCaseFile.CaseNo = caseNo;

                db.HseqCaseFiles.Add(hseqCaseFile);

                ncr.HseqCaseFile = hseqCaseFile;
                ncr.HseqCaseFileID = hseqCaseFile.HseqCaseFileID;

                /////
                hseqCaseFile.HseqRecords.Add(ncr);


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

            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
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
            return View(ncr);
        }

        // POST: Ncrs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,NcrSource,NcrState,DiscrepancyTypeID")] Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ncr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", ncr.HseqCaseFileID);
            ViewBag.DiscrepancyTypeID = new SelectList(db.DiscrepancyTypes, "DiscrepancyTypeID", "Name", ncr.DiscrepancyTypeID);
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

            if (ncr.LinkedRecords != null)
            {

                foreach (HseqRecord linkedRecord in ncr.LinkedRecords)
                {
                    linkedRecord.LinkedRecords.Remove(ncr);
                }

                ncr.LinkedRecords = null;
            }

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
    }
}
