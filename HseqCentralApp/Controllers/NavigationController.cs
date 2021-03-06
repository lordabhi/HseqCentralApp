﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using HseqCentralApp.Helpers;
using HseqCentralApp.Models;
using HseqCentralApp.Services;
using HseqCentralApp.ViewModels;
using AutoMapper;
using System.Diagnostics;

namespace HseqCentralApp.Controllers
{
    public class NavigationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        RecordService _RecordService;

        public static readonly int NCR_TAB_INDEX = 0;
        public static readonly int CAR_TAB_INDEX = 1;
        public static readonly int PAR_TAB_INDEX = 2;
        public static readonly int FIS_TAB_INDEX = 3;
        public static readonly int TASK_TAB_INDEX = 4;
        public static readonly int APPROVAL_TAB_INDEX = 5;
        public static readonly int ALL_ITEM_TAB_INDEX = 6;

        public static readonly string NEW_VIEW_PREFIX = "NewView";
        public static readonly string EDIT_VIEW_PREFIX = "EditView";
        public static readonly string LINKED_VIEW_PREFIX = "LinkedView";

        public NavigationController() 
        {
            _RecordService = new RecordService();
        }

        public NavigationController(RecordService service) 
        {
            _RecordService = service;
        }

        // GET: Navigation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MainContentCallbackPanel()

        // DXCOMMENT: Pass a data model for GridView in the PartialView method's second parameter
        {
            if (DevExpressHelper.IsCallback)
            {
                NavigationFilter.RecordTypeCheckState = null;
                NavigationFilter.ResponsibleAreaCheckState = null;
                NavigationFilter.CoordinatorsCheckState = null;

                NavigationFilter.RecordTypes = null;
                NavigationFilter.ResponsibleAreaIds = null;
                NavigationFilter.CoordinatorIds = null;

                //Checked, Unchecked, Indeterminate 

                NavigationFilter.RecordTypeCheckState = Request.Params["recordTypeCheckState"];
                NavigationFilter.ResponsibleAreaCheckState = Request.Params["responsibleAreaCheckState"];
                NavigationFilter.CoordinatorsCheckState = Request.Params["coordinatorsCheckState"];

                //Record Type
                if (!string.IsNullOrEmpty(Request.Params["recordTypeCheckedNodes"]))
                {
                    string recordTypeNodes = Request.Params["recordTypeCheckedNodes"];

                    NavigationFilter.RecordTypes = recordTypeNodes.Split(',');

                    setActiveTab();
                }

                //Responsible Area Type
                if (!string.IsNullOrEmpty(Request.Params["responsibleAreaCheckedNodes"]))
                {
                    string responsibleAreaNodes = Request.Params["responsibleAreaCheckedNodes"];

                    NavigationFilter.ResponsibleAreaIds = Array.ConvertAll(responsibleAreaNodes.Split(','), int.Parse);
                }

                if (!string.IsNullOrEmpty(Request.Params["coordinatorsCheckedNodes"]))
                {
                    string responsibleAreaNodes = Request.Params["coordinatorsCheckedNodes"];

                    NavigationFilter.CoordinatorIds = Array.ConvertAll(responsibleAreaNodes.Split(','), int.Parse);

                }

                //ViewData["Collapsed"] = false;

                //edit
                if (!string.IsNullOrEmpty(Request.Params["EditRecord"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["currentActiveView"]) && !string.IsNullOrEmpty(Request.Params["recordId"]))
                    {
                        string currentActiveView = Request.Params["currentActiveView"];
                        int recordId = int.Parse(Request.Params["recordId"]);

                        if (currentActiveView.Contains("Task"))
                        {
                            HseqTask record = db.HseqTasks.Find(recordId);
                            HseqTaskEditViewModel HseqTaskEditVM = Mapper.Map<HseqTask, HseqTaskEditViewModel>(record);

                            ViewData["record"] = HseqTaskEditVM;
                            ViewData["currentview"] = "_Task"+EDIT_VIEW_PREFIX;
                        }
                        else if (currentActiveView.Contains("Approval"))
                        {
                            HseqApprovalRequest record = db.HseqApprovalRequests.Find(recordId);
                            HseqApprovalEditViewModel HseqApprovalEditVM = Mapper.Map<HseqApprovalRequest, HseqApprovalEditViewModel>(record);

                            ViewData["record"] = HseqApprovalEditVM;
                            ViewData["currentview"] = "_Approval" + EDIT_VIEW_PREFIX;
                        }

                        ////////////////////////////////////////

                        else if (currentActiveView.Contains("Ncr"))
                        {
                            Ncr record = db.NcrRecords.Find(recordId);
                            NcrEditViewModel ncrEditVM = Mapper.Map<Ncr, NcrEditViewModel>(record);

                            ViewData["record"] = ncrEditVM;
                            ViewData["currentview"] = "_Ncr" + EDIT_VIEW_PREFIX;
                        }
                        else if (currentActiveView.Contains("Car"))
                        {
                            Car record = db.CarRecords.Find(recordId);
                            CarEditViewModel carEditVM = Mapper.Map<Car, CarEditViewModel>(record);

                            ViewData["record"] = carEditVM;
                            ViewData["currentview"] = "_Car" + EDIT_VIEW_PREFIX;
                        }
                        else if (currentActiveView.Contains("Par"))
                        {
                            Par record = db.ParRecords.Find(recordId);
                            ParEditViewModel parEditVM = Mapper.Map<Par, ParEditViewModel>(record);

                            ViewData["record"] = parEditVM;
                            ViewData["currentview"] = "_Par" + EDIT_VIEW_PREFIX;
                        }
                        else if (currentActiveView.Contains("Fis"))
                        {
                            Fis record = db.FisRecords.Find(recordId);
                            FisEditViewModel fisEditVM = Mapper.Map<Fis, FisEditViewModel>(record);

                            ViewData["record"] = fisEditVM;
                            ViewData["currentview"] = "_Fis" + EDIT_VIEW_PREFIX;
                        }
                    }
                }

                //new
                else if (!string.IsNullOrEmpty(Request.Params["NewRecord"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["currentActiveView"]))
                    {
                        string currentActiveView = Request.Params["currentActiveView"];

                        if (currentActiveView.Contains("Ncr"))
                        {
                            Ncr ncr = new Ncr();
                            ncr.CaseNo = _RecordService.GetNextCaseNumber(db);
                            ncr.RecordNo = ncr.CaseNo;
                            ncr.CreatedBy = _RecordService.GetCurrentApplicationUser().FullName;
                            ncr.DateCreated = DateTime.Now;
                            ncr.RecordType = RecordType.NCR;
                            ncr.NcrSource = NcrSource.Internal;
                            ncr.NcrState = NcrState.New;

                            NcrCreateViewModel carVM = Mapper.Map<Ncr, NcrCreateViewModel>(ncr);

                            ViewData["record"] = carVM;
                            ViewData["currentview"] = "_Ncr" + NEW_VIEW_PREFIX;
                        }
                        else if (currentActiveView.Contains("Car"))
                        {
                            Car car = new Car();
                            car.CaseNo = _RecordService.GetNextCaseNumber(db);
                            car.RecordNo = car.CaseNo;
                            car.CreatedBy = _RecordService.GetCurrentApplicationUser().FullName;
                            car.DateCreated = DateTime.Now;
                            car.RecordType = RecordType.CAR;

                            CarCreateViewModel carVM = Mapper.Map<Car, CarCreateViewModel>(car);

                            ViewData["record"] = carVM;
                            ViewData["currentview"] = "_Car" + NEW_VIEW_PREFIX;
                        }
                        else if (currentActiveView.Contains("Par"))
                        {
                            Par par = new Par();
                            par.CaseNo = _RecordService.GetNextCaseNumber(db);
                            par.RecordNo = par.CaseNo;
                            par.CreatedBy = _RecordService.GetCurrentApplicationUser().FullName;
                            par.DateCreated = DateTime.Now;
                            par.RecordType = RecordType.PAR;

                            ParCreateViewModel parVM = Mapper.Map<Par, ParCreateViewModel>(par);

                            ViewData["record"] = parVM;
                            ViewData["currentview"] = "_Par" + NEW_VIEW_PREFIX;
                        }
                        else if (currentActiveView.Contains("Fis"))
                        {
                            Fis fis = new Fis();
                            fis.CaseNo = _RecordService.GetNextCaseNumber(db);
                            fis.RecordNo = fis.CaseNo;
                            fis.CreatedBy = _RecordService.GetCurrentApplicationUser().FullName;
                            fis.DateCreated = DateTime.Now;
                            fis.RecordType = RecordType.FIS;

                            FisCreateViewModel fisVM = Mapper.Map<Fis, FisCreateViewModel>(fis);

                            ViewData["record"] = fisVM;
                            ViewData["currentview"] = "_Fis" + NEW_VIEW_PREFIX;
                        }

                    }
                }

                //// Task ///////////////////
                else if (!string.IsNullOrEmpty(Request.Params["addTask"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["currentActiveView"]) && !string.IsNullOrEmpty(Request.Params["recordId"]))
                    {
                        string currentActiveView = Request.Params["currentActiveView"];
                        int recordId = int.Parse(Request.Params["recordId"]);

                        HseqTask hseqTask = new HseqTask()
                        {
                            Status = TaskStatus.NotStarted
                        };

                        HseqRecord hseqRecord = db.HseqRecords.Find(recordId);
                        hseqTask.HseqRecordID = hseqRecord.HseqRecordID;
                        hseqTask.HseqRecord = hseqRecord;

                        HseqTaskCreateViewModel hseqTaskVM = Mapper.Map<HseqTask, HseqTaskCreateViewModel>(hseqTask);

                        ViewData["record"] = hseqTaskVM;
                        ViewData["currentview"] = "_Task" + NEW_VIEW_PREFIX;
                    }
                }

                //// Approval ///////////////////
                else if (!string.IsNullOrEmpty(Request.Params["addApproval"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["currentActiveView"]) && !string.IsNullOrEmpty(Request.Params["recordId"]))
                    {
                        string currentActiveView = Request.Params["currentActiveView"];
                        int recordId = int.Parse(Request.Params["recordId"]);

                        HseqApprovalRequest hseqApproval = new HseqApprovalRequest()
                        {
                            Response = ApprovalResult.Waiting,
                            Status = ApprovalStatus.Active,
                            DateAssigned = DateTime.Now
                        };

                        HseqRecord hseqRecord = db.HseqRecords.Find(recordId);
                        hseqApproval.HseqRecordID = hseqRecord.HseqRecordID;
                        hseqApproval.HseqRecord = hseqRecord;

                        HseqApprovalCreateViewModel hseqApprovalVM = Mapper.Map<HseqApprovalRequest, HseqApprovalCreateViewModel>(hseqApproval);

                        ViewData["record"] = hseqApprovalVM;
                        ViewData["currentview"] = "_Approval" + NEW_VIEW_PREFIX;
                    }
                }

                //// Linked Record ///////////////////
                else if (!string.IsNullOrEmpty(Request.Params["createLinkedRecord"])
                    && !string.IsNullOrEmpty(Request.Params["linkedRecordDetails"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["recordId"]))
                    {
                        int recordId = int.Parse(Request.Params["recordId"]);

                        var linkedRecordDetails = Request.Params["linkedRecordDetails"].Split('-');
                        var sourceRecord = linkedRecordDetails[0];
                        var targetRecord = linkedRecordDetails[1];

                        //Ncr to Car
                        if (sourceRecord == "NCR") {

                            if (targetRecord == "CAR") {

                                Ncr ncr = db.NcrRecords.Find(recordId);
                                Car car = new Car(ncr) {
                                    CreatedBy = _RecordService.GetCurrentApplicationUser().FullName,
                                    DateCreated = DateTime.Now,
                                    RecordType = RecordType.CAR
                                };

                                CarVM carVM = new CarVM() {
                                    Car = car,
                                    LinkedRecord = true,
                                    SourceRecordId = ncr.HseqRecordID
                                };

                                ViewData["record"] = carVM;
                                ViewData["currentview"] = "_Car" + LINKED_VIEW_PREFIX;
                            }
                        }

                    }
                }

                else
                {
                    ViewData["currentview"] = "_MainContentTabPanel";
                }
        }
            
            return PartialView("_MainContentCallbackPanel");
        }


        public ActionResult CancelEdit()
        {
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaskGridViewAdd(HseqTaskCreateViewModel taskVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (taskVM != null)
                    {

                        HseqTask task = new HseqTask();
                        Mapper.Map(taskVM, task);

                        db.Entry(task).State = EntityState.Added;

                        HseqRecord record = db.HseqRecords.Find(taskVM.HseqRecordID);
                        record.Delegatables.Add(task);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_TaskNewView", taskVM);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_TaskNewView", taskVM);
            }
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TaskGridViewUpdate(HseqTaskEditViewModel taskEditVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (taskEditVM != null)
                    {

                        HseqTask task = db.HseqTasks.Find(taskEditVM.DelegatableID);
                        Mapper.Map(taskEditVM, task);

                        db.Entry(task).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_TaskEditView", taskEditVM);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_TaskEditView", taskEditVM);
            }
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ApprovalGridViewAdd(HseqApprovalCreateViewModel approvalVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (approvalVM != null)
                    {
                        HseqApprovalRequest approval = new HseqApprovalRequest();
                        Mapper.Map(approvalVM, approval);

                        db.Entry(approval).State = EntityState.Added;

                        HseqRecord record = db.HseqRecords.Find(approvalVM.HseqRecordID);
                        record.Delegatables.Add(approval);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_ApprovalNewView", approvalVM);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_ApprovalNewView", approvalVM);
            }
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ApprovalGridViewUpdate(HseqApprovalEditViewModel approvalEditVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (approvalEditVM != null)
                    {
                        if (approvalEditVM.Response == ApprovalResult.Approved
                            || approvalEditVM.Response == ApprovalResult.Rejected)
                        {
                            approvalEditVM.Status = ApprovalStatus.Completed;
                        }else if (approvalEditVM.Response == ApprovalResult.Canceled)
                        {
                            approvalEditVM.Status = ApprovalStatus.Canceled;
                        }

                        HseqApprovalRequest approvalRecord = db.HseqApprovalRequests.Find(approvalEditVM.DelegatableID);
                        Mapper.Map(approvalEditVM, approvalRecord);

                        db.Entry(approvalRecord).State = EntityState.Modified;
                        db.SaveChanges();
                    }
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
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_ApprovalEditView", approvalEditVM);
            }
            return PartialView("_MainContentCallbackPanel");
        }


        [HttpPost, ValidateInput(false)]
       // [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult NcrGridViewUpdate(NcrEditViewModel ncrEditVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ncrEditVM != null)
                    {
                        Ncr ncr = db.NcrRecords.Find(ncrEditVM.HseqRecordID);
                        Mapper.Map(ncrEditVM, ncr);

                        db.Entry(ncr).State = EntityState.Modified;
                        db.SaveChanges();
                    }
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
                catch (InvalidOperationException e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_NcrEditView", ncrEditVM);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_NcrEditView", ncrEditVM);
                }
            }
            else {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_NcrEditView", ncrEditVM);
            }
         //   ModelState.Clear();
            return PartialView("_MainContentCallbackPanel");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CarGridViewUpdate(CarEditViewModel carEditVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (carEditVM != null)
                    {
                        Car car = db.CarRecords.Find(carEditVM.HseqRecordID);
                        Mapper.Map(carEditVM, car);
                        db.Entry(car).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_CarEditView", carEditVM);
            }

            return PartialView("_MainContentCallbackPanel");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult NcrGridViewNew(NcrCreateViewModel ncrVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ncrVM != null)
                    {
                        HseqCaseFile hseqCaseFile;
                        Ncr ncr = new Ncr();
                        Mapper.Map(ncrVM, ncr);

                        ncr = (Ncr)_RecordService.CreateCaseFile(ncr, out hseqCaseFile, db);

                        db.HseqRecords.Add(ncr);
                        db.SaveChanges();

                        //create the folder in Alfresco and return the alfresconoderef
                        //Dummy for now

                        //int alfresconoderef = caseNo;
                        //hseqCaseFile.AlfrescoNoderef = caseNo;

                        //car.AlfrescoNoderef = caseNo;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_NcrNewView", ncrVM);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_NcrNewView", ncrVM);
            }
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CarGridViewNew(CarCreateViewModel carVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (carVM != null)
                    {
                        
                        HseqCaseFile hseqCaseFile;
                        Car car = new Car();
                        Mapper.Map(carVM, car);

                        //Car car = new Car()
                        //{
                        //    CaseNo = carVM.CaseNo,
                        //    RecordNo = carVM.RecordNo,
                        //    Title = carVM.Title,
                        //    Description = carVM.Description,
                        //    JobNumber = carVM.JobNumber,
                        //    DrawingNumber = carVM.DrawingNumber,
                        //    RecordType = carVM.RecordType,
                        //    EnteredBy = carVM.EnteredBy,
                        //    ReportedBy = carVM.ReportedBy,
                        //    CreatedBy = carVM.CreatedBy,
                        //    DateCreated = carVM.DateCreated,
                        //    CoordinatorID = carVM.CoordinatorID
                        //};

                        car = (Car)_RecordService.CreateCaseFile(car, out hseqCaseFile, db);

                        db.HseqRecords.Add(car);
                        db.SaveChanges();

                    }
                }
                
                catch (AutoMapperConfigurationException autoMapperException)
                {

                    throw autoMapperException.InnerException;
                }
                catch (AutoMapperMappingException autoMapperException) {

                    throw autoMapperException.InnerException;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_CarNewView", carVM);
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_CarNewView", carVM);
            }
            return PartialView("_MainContentCallbackPanel");
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult CarGridViewLinked(CarVM carVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (carVM != null)
                    {
                        Car car = carVM.Car;
                        car.CreatedBy = _RecordService.GetCurrentUser().FullName;

                        db.HseqRecords.Add(car);
                        db.SaveChanges();
                        HseqRecord sourceRecord = db.HseqRecords.Find(carVM.SourceRecordId);

                        car.LinkedRecords.Add(sourceRecord);
                        sourceRecord.LinkedRecords.Add(car);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_CarNewView", carVM);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_CarNewView", carVM);
            }
            return PartialView("_MainContentCallbackPanel");
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult ParGridViewNew(Par par)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (par != null)
                    {
                        HseqCaseFile hseqCaseFile;
                        par.CreatedBy = _RecordService.GetCurrentUser().FullName;
                        par = (Par)_RecordService.CreateCaseFile(par, out hseqCaseFile, db);

                        db.HseqRecords.Add(par);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_ParNewView", par);
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_ParNewView", par);
            }

            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ParGridViewUpdate(ParEditViewModel parEditVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (parEditVM != null)
                    {
                        Par par = db.ParRecords.Find(parEditVM.HseqRecordID);
                        Mapper.Map(parEditVM, par);

                        db.Entry(par).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_ParEditView", parEditVM);
            }
                
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FisGridViewNew(FisCreateViewModel fisVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fisVM != null)
                    {
                        Fis fis = new Fis();
                        Mapper.Map(fisVM, fis);

                        HseqCaseFile hseqCaseFile;
                        fis = (Fis)_RecordService.CreateCaseFile(fis, out hseqCaseFile, db);

                        db.HseqRecords.Add(fis);
                        db.SaveChanges();
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);

                            Console.WriteLine("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_FisNewView", fisVM);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_FisNewView", fisVM);
            }

            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FisGridViewUpdate(FisEditViewModel fisEditVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fisEditVM != null)
                    {
                        Fis fis = db.FisRecords.Find(fisEditVM.HseqRecordID);
                        Mapper.Map(fisEditVM, fis);

                        db.Entry(fis).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    ViewData["EditError"] = "Please, correct all errors.";
                    return PartialView("_FisEditView", fisEditVM);
                }

            return PartialView("_MainContentCallbackPanel");
        }

        ////////////////////////////////////////
        
        private void setActiveTab()
        {
          if (!String.IsNullOrEmpty(Request.Params["currentActiveTabIndex"]))
            {
            
                int currentActiveTabIndex = int.Parse(Request.Params["currentActiveTabIndex"]);

                if (currentActiveTabIndex == TASK_TAB_INDEX || currentActiveTabIndex == APPROVAL_TAB_INDEX) {

                    ViewData["activeTabIndex"] = currentActiveTabIndex;
                    return;
                }
            }

            if (NavigationFilter.RecordTypes.Count() > 1)
            {
                ViewData["activeTabIndex"] = ALL_ITEM_TAB_INDEX;
            }
            else if (NavigationFilter.RecordTypes.First().Equals(RecordType.NCR.ToString()))
            {
                ViewData["activeTabIndex"] = NCR_TAB_INDEX;
            }
            else if (NavigationFilter.RecordTypes.First().Equals(RecordType.CAR.ToString()))
            {
                ViewData["activeTabIndex"] = CAR_TAB_INDEX;
            }
            else if (NavigationFilter.RecordTypes.First().Equals(RecordType.PAR.ToString()))
            {
                ViewData["activeTabIndex"] = PAR_TAB_INDEX;
            }
            else if (NavigationFilter.RecordTypes.First().Equals(RecordType.FIS.ToString()))
            {
                ViewData["activeTabIndex"] = FIS_TAB_INDEX;
            }
            else
            {
                ViewData["activeTabIndex"] = ALL_ITEM_TAB_INDEX;
            }
        }

        ////////////////////////////////////////

        public ActionResult RightContentCallbackPanel()
        {
            if (DevExpressHelper.IsCallback)
            {

            }
            return PartialView("_RightContentCallbackPanel");
        }

        ////////////////////////////////////////
        
        public ActionResult LinkedItems()
        {
            string currentActiveView;
            string recordId;

            List<HseqRecord> linkedRecords = new List<HseqRecord>();
            
            HseqRecord record = null;
            if (DevExpressHelper.IsCallback)
            {
                if (!string.IsNullOrEmpty(Request.Params["currentActiveView"]))
                {
                    currentActiveView = Request.Params["currentActiveView"];

                }
                if (!string.IsNullOrEmpty(Request.Params["recordId"]))
                {
                    recordId = Request.Params["recordId"];

                    record = db.HseqRecords.Find(int.Parse(recordId));
                    linkedRecords = record.LinkedRecords.ToList();
                    ViewData["LinkedItems"] = record.LinkedRecords;
                }
            }

            return PartialView("_LinkedItemsPanel", linkedRecords);
        }

        ////////////////////////////////////////
        
        public ActionResult Comments()
        {
            string currentActiveView;
            string recordId = null;
            dynamic recordType = null;

            List<Comment> filteredComments = new List<Comment>();
            
            if (DevExpressHelper.IsCallback)
            {
                if (!string.IsNullOrEmpty(Request.Params["currentActiveView"]))
                {
                    currentActiveView = Request.Params["currentActiveView"];

                    if ((currentActiveView.Contains("Task"))
                        || (currentActiveView.Contains("Approval")))
                    {
                        recordType = CommentSource.Delegatable;
                    }
                    else {
                        recordType = CommentSource.Record;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["recordId"]))
                {
                    recordId = Request.Params["recordId"];
 
                    dynamic record = null;
 
                    if (recordType == CommentSource.Record) {
                        record = db.HseqRecords.Find(int.Parse(recordId));
                        filteredComments = ((HseqRecord)record).Comments.OrderBy(s => s.DateCreated).Reverse().ToList();
                        //ViewData["Comments"] = filteredComments;
                    }
                    else if (recordType == CommentSource.Delegatable)
                    {
                        record = db.Delegatables.Find(int.Parse(recordId));
                        filteredComments = ((Delegatable)record).Comments.OrderBy(s => s.DateCreated).Reverse().ToList();
                        //ViewData["Comments"] = filteredComments;

                    }
                    if (!string.IsNullOrEmpty(Request.Params["newcomment"]))
                    {
                        string newcomment = Request.Params["newcomment"];

                        Comment comment = new Comment()
                        {
                            Content = newcomment,
                            DateCreated = DateTime.Now,
                            CommentSource = recordType,
                            OwnerID = Utils.GetCurrentApplicationUser().HseqUserID
                        };

                        if (recordType == CommentSource.Record) { 
                            comment.HseqRecordID = int.Parse(recordId);
                        }
                        else if (recordType == CommentSource.Delegatable)
                        {
                            comment.DelegatableID = int.Parse(recordId);
                        }

                        db.Comments.Add(comment);
                        db.SaveChanges();
                        
                        if (recordType == CommentSource.Record)
                        {
                            filteredComments = ((HseqRecord)record).Comments.OrderBy(s => s.DateCreated).Reverse().ToList();
                        }
                        else if (recordType == CommentSource.Delegatable) {
                            filteredComments = ((Delegatable)record).Comments.OrderBy(s => s.DateCreated).Reverse().ToList();
                        }
                    }
                }
            }
            return PartialView("_CommentsPanel", filteredComments);
        }


    }

}
