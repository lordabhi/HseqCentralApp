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
using HseqCentralApp.Services;
using HseqCentralApp.ViewModels;

namespace HseqCentralApp.Controllers
{
    public class HseqTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;

        public HseqTasksController()
        {
            _RecordService = new RecordService();
        }

        public HseqTasksController(RecordService service)
        {
            _RecordService = service;
        }

        // GET: HseqTasks1
        public ActionResult Index()
        {
            var delegatables = db.HseqTasks.Include(h => h.Assignee).Include(h => h.HseqRecord).Include(h => h.Owner);
            return View(delegatables.ToList());
        }


        public ActionResult HseqTaskRequest()
        {
            HseqUser user = _RecordService.GetCurrentApplicationUser();
            var ownedRequests = db.HseqTasks.Where(h => h.OwnerID == user.HseqUserID);
            var assignedRequests = db.HseqTasks.Where(h => h.AssigneeID == user.HseqUserID);

            HseqTaskVM hseqTasks = new HseqTaskVM();
            hseqTasks.OwnedTasks= ownedRequests.ToList();
            hseqTasks.AssignedTasks = assignedRequests.ToList();
            return View(hseqTasks);
        }

        // GET: HseqTasks1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqTask hseqTask = db.HseqTasks.Find(id);
            if (hseqTask == null)
            {
                return HttpNotFound();
            }
            return View(hseqTask);
        }

        // GET: HseqTasks1/Create
        public ActionResult Create()
        {
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title");
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            return View();
        }

        // POST: HseqTasks1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqTask hseqTask)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.HseqTasks.Add(hseqTask);
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
            }

            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqTask.HseqRecordID);
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
            return View(hseqTask);
        }

        // GET: HseqTasks1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqTask hseqTask = db.HseqTasks.Find(id);
            if (hseqTask == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.AssigneeID);
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqTask.HseqRecordID);
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.OwnerID);
            return View(hseqTask);
        }

        // POST: HseqTasks1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqTask hseqTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hseqTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.AssigneeID);
            ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqTask.HseqRecordID);
            ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.OwnerID);
            return View(hseqTask);
        }

        // GET: HseqTasks1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HseqTask hseqTask = db.HseqTasks.Find(id);
            if (hseqTask == null)
            {
                return HttpNotFound();
            }
            return View(hseqTask);
        }

        // POST: HseqTasks1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HseqTask hseqTask = db.HseqTasks.Find(id);
            db.HseqTasks.Remove(hseqTask);
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
