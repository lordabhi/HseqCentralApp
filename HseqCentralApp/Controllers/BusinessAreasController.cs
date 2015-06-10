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
    public class BusinessAreasController : Controller
    {
        private HseqCentralAppContext db = new HseqCentralAppContext();

        // GET: BusinessAreas
        public ActionResult Index()
        {
            return View(db.BusinessAreas.ToList());
        }

        // GET: BusinessAreas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessArea businessArea = db.BusinessAreas.Find(id);
            if (businessArea == null)
            {
                return HttpNotFound();
            }
            return View(businessArea);
        }

        // GET: BusinessAreas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BusinessAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BusinessAreaID,Name")] BusinessArea businessArea)
        {
            if (ModelState.IsValid)
            {
                db.BusinessAreas.Add(businessArea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(businessArea);
        }

        // GET: BusinessAreas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessArea businessArea = db.BusinessAreas.Find(id);
            if (businessArea == null)
            {
                return HttpNotFound();
            }
            return View(businessArea);
        }

        // POST: BusinessAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BusinessAreaID,Name")] BusinessArea businessArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(businessArea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(businessArea);
        }

        // GET: BusinessAreas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BusinessArea businessArea = db.BusinessAreas.Find(id);
            if (businessArea == null)
            {
                return HttpNotFound();
            }
            return View(businessArea);
        }

        // POST: BusinessAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BusinessArea businessArea = db.BusinessAreas.Find(id);
            db.BusinessAreas.Remove(businessArea);
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
