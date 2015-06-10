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
    public class DispositionTypesController : Controller
    {
        private HseqCentralAppContext db = new HseqCentralAppContext();

        // GET: DispositionTypes
        public ActionResult Index()
        {
            return View(db.DispositionTypes.ToList());
        }

        // GET: DispositionTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DispositionType dispositionType = db.DispositionTypes.Find(id);
            if (dispositionType == null)
            {
                return HttpNotFound();
            }
            return View(dispositionType);
        }

        // GET: DispositionTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DispositionTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DispositionTypeID,Name")] DispositionType dispositionType)
        {
            if (ModelState.IsValid)
            {
                db.DispositionTypes.Add(dispositionType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dispositionType);
        }

        // GET: DispositionTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DispositionType dispositionType = db.DispositionTypes.Find(id);
            if (dispositionType == null)
            {
                return HttpNotFound();
            }
            return View(dispositionType);
        }

        // POST: DispositionTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DispositionTypeID,Name")] DispositionType dispositionType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dispositionType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dispositionType);
        }

        // GET: DispositionTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DispositionType dispositionType = db.DispositionTypes.Find(id);
            if (dispositionType == null)
            {
                return HttpNotFound();
            }
            return View(dispositionType);
        }

        // POST: DispositionTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DispositionType dispositionType = db.DispositionTypes.Find(id);
            db.DispositionTypes.Remove(dispositionType);
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
