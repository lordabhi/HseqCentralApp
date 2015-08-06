using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using HseqCentralApp.Helpers;
using HseqCentralApp.Models;
using HseqCentralApp.Services;

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
            if (!string.IsNullOrEmpty(Request.Params["edit"]))
            {
                if (!string.IsNullOrEmpty(Request.Params["currentActiveView"]) && !string.IsNullOrEmpty(Request.Params["recordId"]))
                {
                    string currentActiveView = Request.Params["currentActiveView"];
                    int recordId = int.Parse(Request.Params["recordId"]);

                        if (currentActiveView.Contains("Task"))
                        {
                            HseqTask record = db.HseqTasks.Find(recordId);
                            ViewData["record"] = record;
                            ViewData["currentview"] = "_TaskEditView";
                        }
                        else if (currentActiveView.Contains("Approval"))
                        {
                            HseqApprovalRequest record = db.HseqApprovalRequests.Find(recordId);
                            ViewData["record"] = record;
                            ViewData["currentview"] = "_ApprovalEditView";
                        }

                        ////////////////////////////////////////

                        else if (currentActiveView.Contains("Ncr")) {
                            Ncr record = db.NcrRecords.Find(recordId);
                            ViewData["record"] = record;
                            ViewData["currentview"] = "_NcrEditView";
                        }
                        else if (currentActiveView.Contains("Car"))
                        {
                            Car record = db.CarRecords.Find(recordId);
                            ViewData["record"] = record;
                            ViewData["currentview"] = "_CarEditView";
                        }
                        else if (currentActiveView.Contains("Par"))
                        {
                            Par record = db.ParRecords.Find(recordId);
                            ViewData["record"] = record;
                            ViewData["currentview"] = "_ParEditView";
                        }
                        else if (currentActiveView.Contains("Fis"))
                        {
                            Fis record = db.FisRecords.Find(recordId);
                            ViewData["record"] = record;
                            ViewData["currentview"] = "_FisEditView";
                        }
                    }
            }

            //new
            else if (!string.IsNullOrEmpty(Request.Params["new"]))
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

                            ViewData["record"] = ncr;
                            ViewData["currentview"] = "_NcrNewView";
                        }
                    else if (currentActiveView.Contains("Car"))
                    {
                            Car car = new Car();
                            car.CaseNo = _RecordService.GetNextCaseNumber(db);
                            car.RecordNo = car.CaseNo;
                            car.CreatedBy = _RecordService.GetCurrentApplicationUser().FullName;
                            car.DateCreated = DateTime.Now;
                            car.RecordType = RecordType.CAR;

                            ViewData["record"] = car;
                            ViewData["currentview"] = "_CarNewView";
                    }
                    else if (currentActiveView.Contains("Par"))
                    {
                            Par par = new Par();
                            par.CaseNo = _RecordService.GetNextCaseNumber(db);
                            par.RecordNo = par.CaseNo;
                            par.CreatedBy = _RecordService.GetCurrentApplicationUser().FullName;
                            par.DateCreated = DateTime.Now;
                            par.RecordType = RecordType.PAR;

                            ViewData["record"] = par;
                            ViewData["currentview"] = "_ParNewView";
                        }
                    else if (currentActiveView.Contains("Fis"))
                    {
                            Fis fis = new Fis();
                            fis.CaseNo = _RecordService.GetNextCaseNumber(db);
                            fis.RecordNo = fis.CaseNo;
                            fis.CreatedBy = _RecordService.GetCurrentApplicationUser().FullName;
                            fis.DateCreated = DateTime.Now;
                            fis.RecordType = RecordType.FIS;

                            ViewData["record"] = fis;
                            ViewData["currentview"] = "_FisNewView";
                        }
                    }
            }
            //////////////////////////////////////////////////
            else
            {
                ViewData["currentview"] = "_MainContentTabPanel";
            }
        }
         //   Console.WriteLine(NavigationFilter.RecordTypes);
        //   Console.WriteLine(NavigationFilter.ResponsibleAreaIds);
        //   Console.WriteLine(NavigationFilter.CoordinatorIds);
            
            return PartialView("_MainContentCallbackPanel");
        }


        public ActionResult CancelEdit()
        {
            return PartialView("_MainContentCallbackPanel");
        }
        
        [HttpPost, ValidateInput(false)]
        public ActionResult TaskGridViewUpdate(HseqTask item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (item != null)
                    {
                        db.Entry(item).State = EntityState.Modified;
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
                return PartialView("_TaskEditView", item);
            }
            return PartialView("_MainContentCallbackPanel");
        }

        
        [HttpPost, ValidateInput(false)]
        public ActionResult ApprovalGridViewUpdate(HseqApprovalRequest item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (item != null)
                    {
                        db.Entry(item).State = EntityState.Modified;
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
                return PartialView("_ApprovalEditView", item);
            }
            return PartialView("_MainContentCallbackPanel");
        }


        [HttpPost, ValidateInput(false)]
       // [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult NcrGridViewUpdate(HseqCentralApp.Models.Ncr item)
        {
            //   var model = db3.NcrRecords;
            if (ModelState.IsValid)
            {
                try
                {
                    //         var modelItem = model.FirstOrDefault(it => it.HseqRecordID == item.HseqRecordID);
                    if (item != null)
                    {
                        //this.UpdateModel(item);
                        //db.SaveChanges();
                        db.Entry(item).State = EntityState.Modified;
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
            else {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_NcrEditView", item);
            }
            //return PartialView("_NcrGridViewPartial", model.ToList());
         //   ModelState.Clear();
            return PartialView("_MainContentCallbackPanel");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CarGridViewUpdate(HseqCentralApp.Models.Car item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (item != null)
                    {
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_CarEditView", item);
            }
            
            return PartialView("_MainContentCallbackPanel");
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult NcrGridViewNew(Ncr ncr)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ncr != null)
                    {

                        //string caseNo;
                        HseqCaseFile hseqCaseFile;
                        ncr.CreatedBy = _RecordService.GetCurrentUser().FullName;
                        //car = (Ncr)_RecordService.CreateCaseFile(car, out caseNo, out hseqCaseFile, db);

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
                    return PartialView("_NcrNewView", ncr);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine(errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_NcrNewView", ncr);
            }
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CarGridViewNew(HseqCentralApp.Models.Car car)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (car != null)
                    {

                        string caseNo;
                        HseqCaseFile hseqCaseFile;
                        car.CreatedBy = _RecordService.GetCurrentUser().FullName;
                        car = (Car)_RecordService.CreateCaseFile(car, out caseNo, out hseqCaseFile, db);

                        db.HseqRecords.Add(car);
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
                    return PartialView("_CarNewView",car);
                }
            }
            else {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_CarNewView",car);
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
                        string caseNo;
                        HseqCaseFile hseqCaseFile;
                        par.CreatedBy = _RecordService.GetCurrentUser().FullName;
                        par = (Par)_RecordService.CreateCaseFile(par, out caseNo, out hseqCaseFile, db);

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
        public ActionResult ParGridViewUpdate(HseqCentralApp.Models.Par item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (item != null)
                    {
                        db.Entry(item).State = EntityState.Modified;
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
                return PartialView("_ParEditView", item);
            }
                
            return PartialView("_MainContentCallbackPanel");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FisGridViewNew(Fis fis)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fis != null)
                    {
                        string caseNo;
                        HseqCaseFile hseqCaseFile;
                        fis.CreatedBy = _RecordService.GetCurrentUser().FullName;
                        fis = (Fis)_RecordService.CreateCaseFile(fis, out caseNo, out hseqCaseFile, db);

                        db.HseqRecords.Add(fis);
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    return PartialView("_FisNewView", fis);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_FisNewView", fis);
            }

            return PartialView("_MainContentCallbackPanel");
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult FisGridViewUpdate(HseqCentralApp.Models.Fis fis)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (fis != null)
                    {
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
                    return PartialView("_FisEditView", fis);
                }

            return PartialView("_MainContentCallbackPanel");
        }

        ////////////////////////////////////////
        
        /// </summary>
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

                    //ViewData["record"] = db.NcrRecords.Find(int.Parse(recordId));
                    record = db.HseqRecords.Find(int.Parse(recordId));
                    linkedRecords = record.LinkedRecords.ToList();
                    ViewData["LinkedItems"] = record.LinkedRecords;
                }
            }

            return PartialView("_LinkedItemsPanel", linkedRecords);
            //return PartialView("_RightContentPanel");
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

        public ActionResult ExportTo(string OutputFormat)
        {
            //var model = Session["TypedListModel"];
            var model = db.NcrRecords.ToList();

            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    return GridViewExtension.ExportToCsv(GridViewHelper.ExportGridViewSettings, model);
                case "PDF":
                    return GridViewExtension.ExportToPdf(GridViewHelper.ExportGridViewSettings, model);
                case "RTF":
                    return GridViewExtension.ExportToRtf(GridViewHelper.ExportGridViewSettings, model);
                case "XLS":
                    return GridViewExtension.ExportToXls(GridViewHelper.ExportGridViewSettings, model);
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(GridViewHelper.ExportGridViewSettings, model);
                default:
                    return RedirectToAction("Index");
            }
        }

        // [HttpPost]
        //public ActionResult AddNewComment(string selectedMenuItemName)
        //{
        //    return PartialView("_CommentsPanel", db.Comments.ToList());
        //}
    }
}

public static class GridViewHelper
{
    private static GridViewSettings exportGridViewSettings;

    public static GridViewSettings ExportGridViewSettings
    {
        get
        {
            if (exportGridViewSettings == null)
                exportGridViewSettings = CreateExportGridViewSettings();
            return exportGridViewSettings;
        }
    }

    private static GridViewSettings CreateExportGridViewSettings()
    {
        GridViewSettings settings = new GridViewSettings();

        settings.Name = "gvTypedListDataBinding";
        settings.CallbackRouteValues = new { Controller = "Ncrs", Action = "NcrGridViewPartial" };

        settings.KeyFieldName = "HseqRecordID";
        settings.Settings.ShowFilterRow = true;

        settings.Columns.Add("CaseNo");
        settings.Columns.Add("RecordNo");
        settings.Columns.Add("RecordType");
        settings.Columns.Add("Title");
        settings.Columns.Add("Description");

        return settings;
    }
}