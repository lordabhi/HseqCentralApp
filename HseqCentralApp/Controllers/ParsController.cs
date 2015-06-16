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
    public class ParsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pars
        public ActionResult Index()
        {
            var hseqRecords = db.ParRecords.Include(p => p.BusinessArea).Include(p => p.HseqCaseFile);
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
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name");
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID");
            return View();
        }

        // POST: Pars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,BusinessAreaID,status")] Par par)
        {
            if (ModelState.IsValid)
            {
                db.HseqRecords.Add(par);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", par.BusinessAreaID);
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
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", par.BusinessAreaID);
            ViewBag.HseqCaseFileID = new SelectList(db.HseqCaseFiles, "HseqCaseFileID", "HseqCaseFileID", par.HseqCaseFileID);
            return View(par);
        }

        // POST: Pars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqRecordID,AlfrescoNoderef,Title,Description,RecordType,EnteredBy,ReportedBy,QualityCoordinator,HseqCaseFileID,JobNumber,DrawingNumber,BusinessAreaID,status")] Par par)
        {
            if (ModelState.IsValid)
            {
                db.Entry(par).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BusinessAreaID = new SelectList(db.BusinessAreas, "BusinessAreaID", "Name", par.BusinessAreaID);
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
            db.HseqRecords.Remove(par);
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
