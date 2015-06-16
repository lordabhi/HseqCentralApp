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
using System.Diagnostics;

namespace HseqCentralApp.Controllers
{
    public class HseqCaseFilesController : Controller
    {
        //private HseqCentralAppContext db = new HseqCentralAppContext();
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;

        public HseqCaseFilesController() : this(new RecordService()){}

        public HseqCaseFilesController(RecordService service) 
        {
            _RecordService = service;
        }

        // GET: HseqCaseFiles
        public ActionResult Index()
        {
            return View(db.HseqCaseFiles.ToList());
        }

        // GET: HseqCaseFiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqCaseFile hseqCaseFile = db.HseqCaseFiles.Find(id);
            if (hseqCaseFile == null)
            {
                return HttpNotFound();
            }

            var records = from d in db.HseqRecords
                          where d.HseqCaseFileID == id
                           select d;

            ViewBag.LinkedRecords = records;

            return View(hseqCaseFile);
        }

        // GET: HseqCaseFiles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HseqCaseFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqCaseFileID,CaseNo,AlfrescoNoderef")] HseqCaseFile hseqCaseFile)
        {
            if (ModelState.IsValid)
            {
                db.HseqCaseFiles.Add(hseqCaseFile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hseqCaseFile);
        }

        // GET: HseqCaseFiles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqCaseFile hseqCaseFile = db.HseqCaseFiles.Find(id);
            if (hseqCaseFile == null)
            {
                return HttpNotFound();
            }
            return View(hseqCaseFile);
        }

        // POST: HseqCaseFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqCaseFileID,CaseNo,AlfrescoNoderef")] HseqCaseFile hseqCaseFile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hseqCaseFile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hseqCaseFile);
        }

        // GET: HseqCaseFiles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqCaseFile hseqCaseFile = db.HseqCaseFiles.Find(id);
            if (hseqCaseFile == null)
            {
                return HttpNotFound();
            }
            return View(hseqCaseFile);
        }

        // POST: HseqCaseFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HseqCaseFile hseqCaseFile = db.HseqCaseFiles.Find(id);

            Console.WriteLine(hseqCaseFile.HseqRecords);
            Console.WriteLine(hseqCaseFile.HseqRecords.LongCount());

            var tmp = new List<HseqRecord>(hseqCaseFile.HseqRecords);

            foreach (var hsr in tmp)
            {

                //Ncr ncr = db.HseqRecords.Find(hsr.HseqRecordID);
                HseqRecord hr = db.HseqRecords.Find(hsr.HseqRecordID);

                if (hr is Ncr)
                {

                    Ncr ncr = (Ncr)hr;

                    _RecordService.RemoveLinkedRecords(ncr);

                    db.NcrRecords.Remove((Ncr)hr);

                }

                else if (hr is Fis)
                {
                    Fis ncr = (Fis)hr;

                    _RecordService.RemoveLinkedRecords(ncr);

                    db.FisRecords.Remove((Fis)hr);
                }
                else if (hr is Car)
                {
                    Car ncr = (Car)hr;

                    _RecordService.RemoveLinkedRecords(ncr);

                    db.CarRecords.Remove((Car)hr);
                }
                else if (hr is Par)
                {
                    Par ncr = (Par)hr;

                    _RecordService.RemoveLinkedRecords(ncr);

                    db.ParRecords.Remove((Par)hr);
                }
            }

            db.HseqCaseFiles.Remove(hseqCaseFile);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private static void RemoveLinkedRecords(HseqRecord ncr)
        {
            if (ncr.LinkedRecords != null)
            {

                foreach (HseqRecord linkedRecord in ncr.LinkedRecords)
                {
                    linkedRecord.LinkedRecords.Remove(ncr);
                }

                ncr.LinkedRecords = null;
            }
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
