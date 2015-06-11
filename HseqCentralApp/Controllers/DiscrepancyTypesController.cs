using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HseqCentralApp.Models
{
    public class DiscrepancyTypesController : Controller
    {
        //private HseqCentralAppContext db = new HseqCentralAppContext();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DiscrepancyTypes
        public ActionResult Index()
        {
            return View(db.DiscrepancyTypes.ToList());
        }

        // GET: DiscrepancyTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscrepancyType discrepancyType = db.DiscrepancyTypes.Find(id);
            if (discrepancyType == null)
            {
                return HttpNotFound();
            }
            return View(discrepancyType);
        }

        // GET: DiscrepancyTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DiscrepancyTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DiscrepancyTypeID,Name")] DiscrepancyType discrepancyType)
        {
            if (ModelState.IsValid)
            {
                db.DiscrepancyTypes.Add(discrepancyType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(discrepancyType);
        }

        // GET: DiscrepancyTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscrepancyType discrepancyType = db.DiscrepancyTypes.Find(id);
            if (discrepancyType == null)
            {
                return HttpNotFound();
            }
            return View(discrepancyType);
        }

        // POST: DiscrepancyTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DiscrepancyTypeID,Name")] DiscrepancyType discrepancyType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(discrepancyType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(discrepancyType);
        }

        // GET: DiscrepancyTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DiscrepancyType discrepancyType = db.DiscrepancyTypes.Find(id);
            if (discrepancyType == null)
            {
                return HttpNotFound();
            }
            return View(discrepancyType);
        }

        // POST: DiscrepancyTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DiscrepancyType discrepancyType = db.DiscrepancyTypes.Find(id);
            db.DiscrepancyTypes.Remove(discrepancyType);
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
