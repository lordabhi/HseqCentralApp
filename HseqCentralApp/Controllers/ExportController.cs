using DevExpress.Web.Mvc;
using HseqCentralApp.Helpers;
using HseqCentralApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HseqCentralApp.Controllers
{
    public class ExportController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Export
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportTo(string OutputFormat, string currentView)
        {

            GridViewSettings exportSettings = ExportUtils.CreateExportGridViewSettings(currentView);
            if (currentView.Equals("NcrGridView"))
            {
                var filteredAllItemRecords = from record in db.NcrRecords
                                             where NavigationFilter.FilteredNcrRecordIds.Contains(record.HseqRecordID)
                                             select record;

                var model = filteredAllItemRecords.ToList();

                return ExportToType(OutputFormat, exportSettings, model);

            }
            else if (currentView.Equals("CarGridView"))
            {

                var filteredAllItemRecords = from record in db.CarRecords
                                             where NavigationFilter.FilteredCarRecordIds.Contains(record.HseqRecordID)
                                             select record;

                var model = filteredAllItemRecords.ToList();

                return ExportToType(OutputFormat, exportSettings, model);

            }

            else if (currentView.Equals("ParGridView"))
            {

                var filteredAllItemRecords = from record in db.ParRecords
                                             where NavigationFilter.FilteredParRecordIds.Contains(record.HseqRecordID)
                                             select record;

                var model = filteredAllItemRecords.ToList();

                return ExportToType(OutputFormat, exportSettings, model);

            }

            else if (currentView.Equals("FisGridView"))
            {

                var filteredAllItemRecords = from record in db.FisRecords
                                             where NavigationFilter.FilteredFisRecordIds.Contains(record.HseqRecordID)
                                             select record;

                var model = filteredAllItemRecords.ToList();

                return ExportToType(OutputFormat, exportSettings, model);
            }

            else if (currentView.Equals("AllItemsGridView"))
            {
            }

            return RedirectToAction("Index");
        }

        private ActionResult ExportToType(string OutputFormat, GridViewSettings exportSettings, dynamic model)
        {
            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    return GridViewExtension.ExportToCsv(exportSettings, model);
                case "PDF":
                    return GridViewExtension.ExportToPdf(exportSettings, model);
                case "RTF":
                    return GridViewExtension.ExportToRtf(exportSettings, model);
                case "XLS":
                    return GridViewExtension.ExportToXls(exportSettings, model);
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(exportSettings, model);
                default:
                    return RedirectToAction("Index");
            }
        }


    }
}