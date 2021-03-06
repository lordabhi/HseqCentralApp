﻿using HseqCentralApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HseqCentralApp.Controllers;
using AutoMapper;
using HseqCentralApp.App_Start;

namespace HseqCentralApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Database.SetInitializer<ApplicationDbContext>(new HseqCentralInitializer());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Register your View Engine Here.
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();
            //ViewEngines.Engines.Add(new MyViewEngine());

            //Configure Auto mapper settings
            AutoMapperConfig.RegisterMappings();


        }
    }
}
