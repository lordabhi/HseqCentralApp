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
    public class FisCategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FisCategories
        public ActionResult Index()
        {
            return View(db.FisCategories.ToList());
        }

        // GET: FisCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FisCategory fisCategory = db.FisCategories.Find(id);
            if (fisCategory == null)
            {
                return HttpNotFound();
            }
            return View(fisCategory);
        }

        // GET: FisCategories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FisCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] FisCategory fisCategory)
        {
            if (ModelState.IsValid)
            {
                db.FisCategories.Add(fisCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fisCategory);
        }

        // GET: FisCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FisCategory fisCategory = db.FisCategories.Find(id);
            if (fisCategory == null)
            {
                return HttpNotFound();
            }
            return View(fisCategory);
        }

        // POST: FisCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] FisCategory fisCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fisCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fisCategory);
        }

        // GET: FisCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FisCategory fisCategory = db.FisCategories.Find(id);
            if (fisCategory == null)
            {
                return HttpNotFound();
            }
            return View(fisCategory);
        }

        // POST: FisCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FisCategory fisCategory = db.FisCategories.Find(id);
            db.FisCategories.Remove(fisCategory);
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
