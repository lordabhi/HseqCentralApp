using System.Web;
using System.Web.Mvc;

namespace HseqCentralApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //Global filter for Authentication
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
