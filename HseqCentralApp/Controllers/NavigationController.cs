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

            Console.WriteLine(NavigationFilter.RecordTypes);
            Console.WriteLine(NavigationFilter.ResponsibleAreaIds);
            Console.WriteLine(NavigationFilter.CoordinatorIds);

            return PartialView("_MainContentCallbackPanel");
        }

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

        public ActionResult LinkedItems()
        {

            string currentActiveView;
            string recordId;

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
                    ViewData["LinkedItems"] = record.LinkedRecords;
                }
            }

            return PartialView("_LinkedItemsPanel");
            //return PartialView("_RightContentPanel");

        }

        public ActionResult Comments()
        {

            string currentActiveView;
            string recordId = null;

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
                    ViewData["Comments"] = record.Comments.OrderBy(s=> s.DateCreated).Reverse();

                }

                if (!string.IsNullOrEmpty(Request.Params["newcomment"]))
                {
                    string newcomment = Request.Params["newcomment"];

                    Comment comment = new Comment() { 
                        Content = newcomment, 
                        DateCreated = DateTime.Now,
                        CommentSource = CommentSource.Record,
                        HseqRecordID = int.Parse(recordId), 
                        OwnerID = Utils.GetCurrentApplicationUser().HseqUserID
                    };

                    db.Comments.Add(comment);
                    db.SaveChanges();
                }


            }
            return PartialView("_CommentsPanel", db.Comments.ToList());
        }

         [HttpPost]
        public ActionResult AddNewComment(string selectedMenuItemName)
        {



            return PartialView("_CommentsPanel", db.Comments.ToList());
        }


    }
}