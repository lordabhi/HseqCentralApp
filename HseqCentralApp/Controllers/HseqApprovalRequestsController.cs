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

namespace HseqCentralApp.Controllers
{
    public class HseqApprovalRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        RecordService _RecordService;
        DelegatableService _DelegatableService;

        public HseqApprovalRequestsController()
        {
            _RecordService = new RecordService();
            _DelegatableService = new DelegatableService();
        }

        public HseqApprovalRequestsController(RecordService service)
        {
            _RecordService = service;
        }
        
        public HseqApprovalRequestsController(DelegatableService service)
        {
            _DelegatableService = service;
        }
        // GET: HseqApprovalRequests1
        public ActionResult Index()
        {
            var delegatables = db.HseqApprovalRequests.Include(h => h.Assignee).Include(h => h.HseqRecord).Include(h => h.Owner);
            return View(delegatables.ToList());
        }


        public ActionResult OpenAction()
        {
            HseqUser user = _RecordService.GetCurrentApplicationUser();
            //var delegatables = db.HseqApprovalRequests.Include(h => h.Assignee).Include(h => h.HseqRecord).Include(h => h.Owner);
            var ownedRequests = db.HseqApprovalRequests.Where(h => h.OwnerID == user.HseqUserID && h.Status==ApprovalStatus.Active);
            var assignedRequests = db.HseqApprovalRequests.Where(h => h.AssigneeID == user.HseqUserID && h.Status == ApprovalStatus.Active);

            HseqApprovalRequestVM approvalRequests = new HseqApprovalRequestVM();
            approvalRequests.OwnedRequests = ownedRequests.ToList();
            approvalRequests.AssignedRequests = assignedRequests.ToList();
            return View(approvalRequests);
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
        public ActionResult Create1()
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
        public ActionResult Create1([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqApprovalRequest hseqApprovalRequest)
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
        public ActionResult Edit1(int? id)
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
        public ActionResult Edit1([Bind(Include = "DelegatableID,OwnerID,AssigneeID,DateAssigned,Title,Description,DueDate,HseqRecordID,Status,Response,ResponseDate,ResponseComment")] HseqApprovalRequest hseqApprovalRequest)
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

        // //////////////// Abhi /////////////////////////////////////////////////////////////////////////////////////////
        // GET: Ncrs/Edit/5
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

            HseqApprovalRequest approvalRequest = new HseqApprovalRequest() { 
                DueDate = DateTime.Now.AddDays(14), 
                OwnerID = Utils.GetCurrentApplicationUser(db).HseqUserID,
                Status = ApprovalStatus.Active,
                HseqRecordID = hseqRecord.HseqRecordID
            };

            DelegatableVM ncrVM = new DelegatableVM() { 
                HseqRecord = hseqRecord, 
                HseqApprovalRequest = approvalRequest, 
                ApprovalOwners = db.HseqUsers 
            };

            return View(ncrVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DelegatableVM ncrVM)
        {

            HseqRecord hseqRecord = null;
            if (ModelState.IsValid)
            {
                hseqRecord = db.HseqRecords.Find(ncrVM.HseqApprovalRequest.HseqRecordID);
                ncrVM.HseqRecord = hseqRecord;

                HseqApprovalRequest hseqApprovalRequest = ncrVM.HseqApprovalRequest;

                if (hseqRecord.RecordType == RecordType.NCR)
                {
                    Ncr ncr = db.NcrRecords.Find(ncrVM.HseqRecord.HseqRecordID);
                    ncr.NcrState = NcrState.DispositionProposed;
                    ncr.DateLastUpdated = DateTime.Now;
                }
                else {
                    hseqRecord.DateLastUpdated = DateTime.Now;
                }

                HseqApprovalRequest approvalRequest = _DelegatableService.AddHseqApprovalRequest(hseqRecord, hseqApprovalRequest, db);

                //ncr.Delegatables.Add(approvalRequest);

                db.SaveChanges();
                return RedirectToAction("OpenAction", "HseqApprovalRequests");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
            }

            return View(ncrVM);

        }

        /////////////////////////////////////////////////////////////////

        public ActionResult Edit(int? recordId)
        {
            if (recordId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HseqApprovalRequest approvalRequest = db.HseqApprovalRequests.Find(recordId);

            if (approvalRequest == null)
            {
                return HttpNotFound();
            }

            //Ncr ncr = db.NcrRecords.Find(recordId);
            HseqRecord hseqRecord = db.HseqRecords.Find(approvalRequest.HseqRecordID);

            if (hseqRecord == null)
            {
                return HttpNotFound();
            }

            //NcrVM ncrVM = new NcrVM();
            DelegatableVM ncrVM = new DelegatableVM() { 
                HseqRecord = hseqRecord, 
                HseqApprovalRequest = approvalRequest, 
                ApprovalOwners = db.HseqUsers 
            };

            return View(ncrVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DelegatableVM ncrVM)
        {

            Ncr ncr = null;
            if (ModelState.IsValid)
            {
                HseqRecord hseqRecord = db.HseqRecords.Find(ncrVM.HseqApprovalRequest.HseqRecordID);
                //ncrVM.Ncr = ncr;

                HseqApprovalRequest hseqApprovalRequest = ncrVM.HseqApprovalRequest;

                //Update Ncr Status
                if (hseqRecord.RecordType == RecordType.NCR)
                {
                    ncr = db.NcrRecords.Find(ncrVM.HseqApprovalRequest.HseqRecordID);
                    if (hseqApprovalRequest.Response == ApprovalResult.Approved)
                    {
                        ncr.NcrState = NcrState.DispositionApproved;
                        ncr.DateLastUpdated = DateTime.Now;
                        db.Entry(ncr).State = EntityState.Modified;
                    }
                    else if (hseqApprovalRequest.Response == ApprovalResult.Rejected)
                    {
                        ncr.NcrState = NcrState.DispositionRejected;
                        ncr.DateLastUpdated = DateTime.Now;
                        db.Entry(ncr).State = EntityState.Modified;
                    }
                }

                db.Entry(hseqApprovalRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("OpenAction", "HseqApprovalRequests");
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
        public ActionResult ApprovalGridViewPartial()
        {
            //var model = db1.HseqApprovalRequests;
            var model = NavigationUtils.GetFilteredApprovalRequests();
            AllItemsVM.ApprovalRecords = model;
            return PartialView("~/Views/Shared/_ApprovalGridViewPartial.cshtml", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ApprovalGridViewPartialAddNew(HseqCentralApp.Models.HseqTask item)
        {
            var model = db1.HseqApprovalRequests;
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
            return PartialView("~/Views/Shared/_ApprovalGridViewPartial.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ApprovalGridViewPartialUpdate(HseqCentralApp.Models.HseqTask item)
        {
            var model = db1.HseqApprovalRequests;
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
            return PartialView("~/Views/Shared/_ApprovalGridViewPartial.cshtml", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ApprovalGridViewPartialDelete(System.Int32 DelegatableID)
        {
            var model = db1.HseqApprovalRequests;
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
            return PartialView("~/Views/Shared/_ApprovalGridViewPartial.cshtml", model);
        }

        public ActionResult _ApprovalChartContainer()
        {
            var model = db1.HseqApprovalRequests;
            return PartialView("_ApprovalChartContainer", model.ToList());
        }





    }
}
