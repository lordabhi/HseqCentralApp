using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HseqCentralApp.Models;

namespace HseqCentralApp.Controllers
{
    public class HseqApprovalRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HseqApprovalRequests1
        public ActionResult Index()
        {
            var delegatables = db.HseqApprovalRequests.Include(h => h.Assignee).Include(h => h.HseqRecord).Include(h => h.Owner);
            return View(delegatables.ToList());
        }

        // GET: HseqApprovalRequests1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqApprovalRequest hseqApprovalRequest = db.HseqApprovalRequests.Find(id);
            if (hseqApprovalRequest == null)
            {
                return HttpNotFound();
            }
            return View(hseqApprovalRequest);
        }

        // GET: HseqApprovalRequests1/Create
        public ActionResult Create()
        {
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title");
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            return View();
        }

        // POST: HseqApprovalRequests1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqApprovalRequest hseqApprovalRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.HseqApprovalRequests.Add(hseqApprovalRequest);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    // Retrieve the error messages as a list of strings.
                    var errorMessages = ex.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);

                    // Join the list to a single string.
                    var fullErrorMessage = string.Join("; ", errorMessages);

                    // Combine the original exception message with the new one.
                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                    // Throw a new DbEntityValidationException with the improved exception message.
                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                }

                //catch (DbEntityValidationException e)
                //{
                //    foreach (var eve in e.EntityValidationErrors)
                //    {
                //        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                //        Console.WriteLine();
                //        foreach (var ve in eve.ValidationErrors)
                //        {
                //            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);

                //            Console.WriteLine();
                //        }
                //    }
                //  //  throw;
                //}
            }

            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqApprovalRequest.HseqRecordID);
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            return View(hseqApprovalRequest);
        }

        // GET: HseqApprovalRequests1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqApprovalRequest hseqApprovalRequest = db.HseqApprovalRequests.Find(id);
            if (hseqApprovalRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqApprovalRequest.AssigneeID);
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqApprovalRequest.HseqRecordID);
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqApprovalRequest.OwnerID);
            return View(hseqApprovalRequest);
        }

        // POST: HseqApprovalRequests1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqApprovalRequest hseqApprovalRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hseqApprovalRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqApprovalRequest.AssigneeID);
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqApprovalRequest.HseqRecordID);
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqApprovalRequest.OwnerID);
            return View(hseqApprovalRequest);
        }

        // GET: HseqApprovalRequests1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqApprovalRequest hseqApprovalRequest = db.HseqApprovalRequests.Find(id);
            if (hseqApprovalRequest == null)
            {
                return HttpNotFound();
            }
            return View(hseqApprovalRequest);
        }

        // POST: HseqApprovalRequests1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HseqApprovalRequest hseqApprovalRequest = db.HseqApprovalRequests.Find(id);
            db.HseqApprovalRequests.Remove(hseqApprovalRequest);
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
