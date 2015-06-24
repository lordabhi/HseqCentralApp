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
    public class HseqUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HseqUsers
        public ActionResult Index()
        {
            var hseqUsers = db.HseqUsers.Include(h => h.User);
            return View(hseqUsers.ToList());
        }

        // GET: HseqUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqUser hseqUser = db.HseqUsers.Find(id);
            if (hseqUser == null)
            {
                return HttpNotFound();
            }
            return View(hseqUser);
        }

        // GET: HseqUsers/Create
        public ActionResult Create()
        {
            var remainingUsers = UsersForDispositionsApproval();

            ViewBag.UserID = new SelectList(remainingUsers, "Id", "FullName");
            return View();
        }

        private IQueryable<ApplicationUser> UsersForDispositionsApproval()
        {
            var remainingUsers = from u in db.Users
                                 where !(from a in db.HseqUsers
                                         where a.UserID == u.Id
                                         select a.UserID).Contains(u.Id)
                                 select u;
            return remainingUsers;
        }

        // POST: HseqUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "HseqUserID,UserID,Approver,Owner,Assignee,Coordinator")] HseqUser hseqUser)
        {
            if (ModelState.IsValid)
            {
                db.HseqUsers.Add(hseqUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var remainingUsers = UsersForDispositionsApproval();

            ViewBag.UserID = new SelectList(remainingUsers, "Id", "FullName", hseqUser.UserID);
            return View(hseqUser);
        }

        // GET: HseqUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqUser hseqUser = db.HseqUsers.Find(id);
            if (hseqUser == null)
            {
                return HttpNotFound();
            }

            var remainingUsers = UsersForDispositionsApproval();

            ViewBag.UserID = new SelectList(db.Users, "Id", "FullName", hseqUser.UserID);

            return View(hseqUser);
        }

        // POST: HseqUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HseqUserID,UserID,Approver,Owner,Assignee,Coordinator")] HseqUser hseqUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hseqUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            var remainingUsers = UsersForDispositionsApproval();

            ViewBag.UserID = new SelectList(db.Users, "Id", "FullName", hseqUser.UserID);
            return View(hseqUser);
        }

        // GET: HseqUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqUser hseqUser = db.HseqUsers.Find(id);
            if (hseqUser == null)
            {
                return HttpNotFound();
            }
            return View(hseqUser);
        }

        // POST: HseqUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HseqUser hseqUser = db.HseqUsers.Find(id);
            db.HseqUsers.Remove(hseqUser);
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
