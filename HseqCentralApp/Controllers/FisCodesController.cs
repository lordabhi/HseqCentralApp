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
    public class FisCodesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FisCodes
        public ActionResult Index()
        {
            var districts = db.FisCodes.Include(f => f.FisCategory);
            return View(districts.ToList());
        }

        // GET: FisCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FisCode fisCode = db.FisCodes.Find(id);
            if (fisCode == null)
            {
                return HttpNotFound();
            }
            return View(fisCode);
        }

        // GET: FisCodes/Create
        public ActionResult Create()
        {
            ViewBag.FisCategoryId = new SelectList(db.FisCategories, "Id", "Name");
            return View();
        }

        // POST: FisCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CodeName,FisCategoryId")] FisCode fisCode)
        {
            if (ModelState.IsValid)
            {
                db.FisCodes.Add(fisCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FisCategoryId = new SelectList(db.FisCategories, "Id", "Name", fisCode.FisCategoryId);
            return View(fisCode);
        }

        // GET: FisCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FisCode fisCode = db.FisCodes.Find(id);
            if (fisCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.FisCategoryId = new SelectList(db.FisCategories, "Id", "Name", fisCode.FisCategoryId);
            return View(fisCode);
        }

        // POST: FisCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CodeName,FisCategoryId")] FisCode fisCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fisCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FisCategoryId = new SelectList(db.FisCategories, "Id", "Name", fisCode.FisCategoryId);
            return View(fisCode);
        }

        // GET: FisCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FisCode fisCode = db.FisCodes.Find(id);
            if (fisCode == null)
            {
                return HttpNotFound();
            }
            return View(fisCode);
        }

        // POST: FisCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FisCode fisCode = db.FisCodes.Find(id);
            db.FisCodes.Remove(fisCode);
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
