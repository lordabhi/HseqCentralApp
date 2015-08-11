using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Web.Mvc;


public static class ExportUtils
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

    public static GridViewSettings CreateExportGridViewSettings()
    {
        GridViewSettings settings = new GridViewSettings()
        {
            Name = "gvTypedListDataBinding",
            CallbackRouteValues = new
            { Controller = "Ncrs", Action = "NcrGridViewPartial" },
            KeyFieldName = "HseqRecordID"
        };
        settings.Settings.ShowFilterRow = true;
        //settings.SettingsExport.ExportedRowType = DevExpress.Web.GridViewExportedRowType.Selected;
        settings.Columns.Add("CaseNo");
        settings.Columns.Add("RecordNo");
        settings.Columns.Add("RecordType");
        settings.Columns.Add("Title");
        settings.Columns.Add("Description");

        return settings;
    }


    public static GridViewSettings CreateExportGridViewSettings(string currentView)
    {
        GridViewSettings settings = new GridViewSettings() {
            Name = currentView
        };

        if (currentView.Equals("NcrGridView"))
        {
            settings.CallbackRouteValues = new { Controller = "Ncrs", Action = "NcrGridViewPartial" };
        }
        else if (currentView.Equals("CarGridView"))
        {
            settings.CallbackRouteValues = new { Controller = "Cars", Action = "CarGridViewPartial" };
        }
        else if (currentView.Equals("ParGridView"))
        {
            settings.CallbackRouteValues = new { Controller = "Pars", Action = "ParGridViewPartial" };
        }
        else if (currentView.Equals("FisGridView"))
        {
            settings.CallbackRouteValues = new { Controller = "Fis", Action = "FisGridViewPartial" };
        }
        else if (currentView.Equals("AllItemsGridView"))
        {
            settings.CallbackRouteValues = new { Controller = "Home", Action = "AllItemsGridViewPartial" };
        }


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