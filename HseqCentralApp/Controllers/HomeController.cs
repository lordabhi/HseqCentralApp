using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using HseqCentralApp.Helpers;
using HseqCentralApp.Models;
using HseqCentralApp.ViewModels;

namespace HseqCentralApp.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            dynamic email = new Email("Example");
            email.To = "abhishek.khaitan@waiward.com";
            email.FunnyLink = "google.com";
            
            try {
                //email.Send();
            }catch(Exception e){
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("test"));

            //ViewBag.MIList = db.MenuItems.ToList();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        /////////////////////////////////////////////////////////////////////////////////////////


        public ActionResult RecordTypeTreeViewPartial()
        {
            return PartialView("_RecordTypeTreeViewPartial");
        }

        public ActionResult ResponsibleAreaTreeViewPartial()
        {
            return PartialView("_ResponsibleAreaTreeViewPartial");
        }

        public ActionResult CoordinatorTreeViewPartial()
        {
            return PartialView("_CoordinatorTreeViewPartial");
        }



        public ActionResult HseqHome()
        {
            return PartialView("HseqHome");
        }


        HseqCentralApp.Models.ApplicationDbContext db3 = new HseqCentralApp.Models.ApplicationDbContext();

        public ActionResult _AllItemsChartContainer()
        {
            var model = db3.HseqRecords;
            return PartialView("_AllItemsChartContainer", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult _AllItemsGridViewPartial()
        {
            //var model = db3.HseqRecords;
            var model = NavigationUtils.GetFilteredAllItems();
            return PartialView("_AllItemsGridViewPartial", model.ToList());
        }

    }
}

public enum HeaderViewRenderMode { Full, Title }