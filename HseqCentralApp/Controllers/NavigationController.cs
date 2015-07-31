using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using HseqCentralApp.Helpers;
using HseqCentralApp.Models;

namespace HseqCentralApp.Controllers
{
    public class NavigationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public static readonly int NCR_TAB_INDEX = 0;
        public static readonly int CAR_TAB_INDEX = 1;
        public static readonly int PAR_TAB_INDEX = 2;
        public static readonly int FIS_TAB_INDEX = 3;
        public static readonly int TASK_TAB_INDEX = 4;
        public static readonly int APPROVAL_TAB_INDEX = 5;
        public static readonly int ALL_ITEM_TAB_INDEX = 6;

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
                ViewData["Collapsed"] = false;

            }

            if (!string.IsNullOrEmpty(Request.Params["edit"]))
            {
                ViewData["currentview"] = "_EditView";
                //return PartialView(ViewData["currentview"]);
            }
            else {
                ViewData["currentview"] = "_MainContentTabPanel";
            }
            
         //   Console.WriteLine(NavigationFilter.RecordTypes);
        //   Console.WriteLine(NavigationFilter.ResponsibleAreaIds);
        //   Console.WriteLine(NavigationFilter.CoordinatorIds);
            

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