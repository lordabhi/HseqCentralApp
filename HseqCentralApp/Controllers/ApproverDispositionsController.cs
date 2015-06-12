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
    public class ApproverDispositionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApproverDispositions
        public ActionResult Index()
        {
            var approverDispositions = db.ApproverDispositions.Include(a => a.Approver);
            return View(approverDispositions.ToList());
        }

        // GET: ApproverDispositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApproverDisposition approverDisposition = db.ApproverDispositions.Find(id);
            if (approverDisposition == null)
            {
                return HttpNotFound();
            }
            return View(approverDisposition);
        }

        // GET: ApproverDispositions/Create
        public ActionResult Create()
        {


            var remainingUsers = UsersForDispositionsApproval();

            ViewBag.ApproverID = new SelectList(remainingUsers, "Id", "FullName");
            return View();
        }

        private IQueryable<ApplicationUser> UsersForDispositionsApproval()
        {
            var remainingUsers = from u in db.Users
                                 where !(from a in db.ApproverDispositions
                                         where a.ApproverID == u.Id
                                         select a.ApproverID).Contains(u.Id)
                                 select u;
            return remainingUsers;
        }

        // POST: ApproverDispositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApproverDispositionID,ApproverID")] ApproverDisposition approverDisposition)
        {
            if (ModelState.IsValid)
            {
                db.ApproverDispositions.Add(approverDisposition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var remainingUsers = UsersForDispositionsApproval();

            ViewBag.ApproverID = new SelectList(remainingUsers, "Id", "FirstName", approverDisposition.ApproverID);
            return View(approverDisposition);
        }

        // GET: ApproverDispositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApproverDisposition approverDisposition = db.ApproverDispositions.Find(id);
            if (approverDisposition == null)
            {
                return HttpNotFound();
            }
            var remainingUsers = UsersForDispositionsApproval();
            ViewBag.ApproverID = new SelectList(remainingUsers, "Id", "FirstName", approverDisposition.ApproverID);
            return View(approverDisposition);
        }

        // POST: ApproverDispositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApproverDispositionID,ApproverID")] ApproverDisposition approverDisposition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(approverDisposition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var remainingUsers = UsersForDispositionsApproval();
            ViewBag.ApproverID = new SelectList(remainingUsers, "Id", "FirstName", approverDisposition.ApproverID);
            return View(approverDisposition);
        }

        // GET: ApproverDispositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApproverDisposition approverDisposition = db.ApproverDispositions.Find(id);
            if (approverDisposition == null)
            {
                return HttpNotFound();
            }
            return View(approverDisposition);
        }

        // POST: ApproverDispositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ApproverDisposition approverDisposition = db.ApproverDispositions.Find(id);
            db.ApproverDispositions.Remove(approverDisposition);
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
