using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HseqCentralApp.Controllers
{
    public class HomeController : Controller
    {
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
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("test"));

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
    }
}