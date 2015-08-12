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
using HseqCentralApp.Helpers;
using HseqCentralApp.Models;
using HseqCentralApp.Services;
using HseqCentralApp.ViewModels;
using DevExpress.Web.Mvc;

namespace HseqCentralApp.Controllers
{
    public class HseqTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;
        DelegatableService _DelegatableService;

        public HseqTasksController()
        {
            _RecordService = new RecordService();
            _DelegatableService = new DelegatableService();
        }

        public HseqTasksController(RecordService service)
        {
            _RecordService = service;
        }

        public HseqTasksController(DelegatableService service)
        {
            _DelegatableService = service;
        }

        // GET: HseqTasks1
        public ActionResult Index()
        {
            var delegatables = db.HseqTasks.Include(h => h.Assignee).Include(h => h.HseqRecord).Include(h => h.Owner);
            return View(delegatables.ToList());
        }

        public ActionResult OpenAction()
        {
            HseqUser user = _RecordService.GetCurrentApplicationUser();
            var ownedRequests = db.HseqTasks.Where(h => h.OwnerID == user.HseqUserID && (h.Status == TaskStatus.Active || h.Status == TaskStatus.NotStarted));
            var assignedRequests = db.HseqTasks.Where(h => h.AssigneeID == user.HseqUserID && (h.Status == TaskStatus.Active || h.Status == TaskStatus.NotStarted));

            HseqTaskVM hseqTasks = new HseqTaskVM() { 
                OwnedTasks = ownedRequests.ToList(), 
                AssignedTasks = assignedRequests.ToList() 
            };
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

        //// GET: HseqTasks1/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
        //    ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title");
        //    ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
        //    return View();
        //}

        //// POST: HseqTasks1/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqTask hseqTask)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            db.HseqTasks.Add(hseqTask);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        catch (DbEntityValidationException ex)
        //        {
        //            // Retrieve the error messages as a list of strings.
        //            var errorMessages = ex.EntityValidationErrors
        //                    .SelectMany(x => x.ValidationErrors)
        //                    .Select(x => x.ErrorMessage);

        //            // Join the list to a single string.
        //            var fullErrorMessage = string.Join("; ", errorMessages);

        //            // Combine the original exception message with the new one.
        //            var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

        //            // Throw a new DbEntityValidationException with the improved exception message.
        //            throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
        //        }
        //    }

        //    ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
        //    ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqTask.HseqRecordID);
        //    ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");
        //    return View(hseqTask);
        //}

        //// GET: HseqTasks1/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    HseqTask hseqTask = db.HseqTasks.Find(id);
        //    if (hseqTask == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.AssigneeID);
        //    ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqTask.HseqRecordID);
        //    ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.OwnerID);
        //    return View(hseqTask);
        //}

        //// POST: HseqTasks1/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqTask hseqTask)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(hseqTask).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("OpenAction");
        //    }
        //    ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.AssigneeID);
        //    ViewBag.HseqRecordID = new SelectList(db.HseqRecords, "HseqRecordID", "Title", hseqTask.HseqRecordID);
        //    ViewBag.OwnerID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", hseqTask.OwnerID);
        //    return View(hseqTask);
        //}

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

        ///////////////////////////////////////////////////////////

        public ActionResult Create(int? recordId)
        {
            if (recordId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Ncr ncr = db.NcrRecords.Find(recordId);
            HseqRecord hseqRecord = db.HseqRecords.Find(recordId);

            if (hseqRecord == null)
            {
                return HttpNotFound();
            }

            HseqTask hseqTask = new HseqTask() { 
                DueDate = DateTime.Now.AddDays(14), 
                OwnerID = Utils.GetCurrentApplicationUser(db).HseqUserID, 
                Status = TaskStatus.NotStarted, 
                HseqRecordID = hseqRecord.HseqRecordID
            };

            DelegatableVM delegatableVM = new DelegatableVM()
            {
                HseqRecord = hseqRecord,
                HseqTask = hseqTask, /*hseqTask.AssigneeID = Utils.GetCurrentApplicationUser(db).HseqUserID;*/
                TaskOwners = db.HseqUsers 
            };

            //ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name", ncr.DispositionTypeID);
            //ViewBag.AssigneeID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqTask.AssigneeID);
            //ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName", ncrVM.HseqTask.OwnerID);

            return View(delegatableVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DelegatableVM ncrVM)
        {

            HseqRecord ncr = null;
            if (ModelState.IsValid)
            {
                //ncr = db.NcrRecords.Find(ncrVM.HseqTask.HseqRecordID);
                ncr = db.HseqRecords.Find(ncrVM.HseqTask.HseqRecordID);
                ncrVM.HseqRecord = ncr;

                HseqTask hseqTask = ncrVM.HseqTask;

                _DelegatableService.AddHseqTaskRequest(ncr, hseqTask, db);

                db.SaveChanges();
                return RedirectToAction("OpenAction", "HseqTasks");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View(ncrVM);
        }

        //Type needs to come in
        public ActionResult Edit(int? recordId)
        {
            if (recordId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HseqTask hseqTask = db.HseqTasks.Find(recordId);
            

            if (hseqTask == null)
            {
                return HttpNotFound();
            }

            //Ncr hseqRecord = db.NcrRecords.Find(hseqTask.HseqRecordID);
            HseqRecord hseqRecord = db.HseqRecords.Find(hseqTask.HseqRecordID);
            if (hseqRecord == null)
            {
                return HttpNotFound();
            }


            DelegatableVM delegatableVM = new DelegatableVM()
            {
                HseqRecord = hseqRecord,
                HseqTask = hseqTask, /*hseqTask.AssigneeID = Utils.GetCurrentApplicationUser(db).HseqUserID;*/
                TaskOwners = db.HseqUsers
            };

            return View(delegatableVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DelegatableVM ncrVM)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ncrVM.HseqTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("OpenAction");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View(ncrVM);
        }
        // //////////////////////////////////////////


        HseqCentralApp.Models.ApplicationDbContext db1 = new HseqCentralApp.Models.ApplicationDbContext();

        [ValidateInput(false)]
        public ActionResult TaskGridViewPartial()
        {
            //var model = db1.HseqTasks;
            var model = NavigationUtils.GetFilteredTasks();
            AllItemsVM.TaskRecords = model;
            return PartialView("~/Views/Shared/_TaskGridView.cshtml", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaskGridViewPartialAddNew(HseqCentralApp.Models.HseqTask item)
        {
            var model = db1.HseqTasks;
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("~/Views/Shared/_TaskGridView.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TaskGridViewPartialUpdate(HseqCentralApp.Models.HseqTask item)
        {
            var model = db1.HseqTasks;
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("~/Views/Shared/_TaskGridView.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TaskGridViewPartialDelete(System.Int32 DelegatableID)
        {
            var model = db1.HseqTasks;
            if (DelegatableID >= 0)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("~/Views/Shared/_TaskGridView.cshtml", model);
        }

        public ActionResult _TaskChartContainer()
        {
            var model = db1.HseqTasks;
            return PartialView("_TaskChartContainer", model.ToList());
        }
    }
}
