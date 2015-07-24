using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.ViewModels;

namespace HseqCentralApp.Helpers
{
    public static class NavigationFilter
    {

        public static string RecordTypeCheckState { get; set; }
        public static string ResponsibleAreaCheckState { get; set; }
        public static string CoordinatorsCheckState { get; set; }

        public static IEnumerable<string> RecordTypes { get; set; }
        public static IEnumerable<int> CoordinatorIds { get; set; }
        public static IEnumerable<int> ResponsibleAreaIds { get; set; }
        public static IEnumerable<int> ProjectIds { get; set; }
        public static IEnumerable<int> JobIds { get; set; }

        //Derived Results
        public static List<int> FilteredNcrRecordIds = new List<int>();
        public static List<int> FilteredCarRecordIds = new List<int>();
        public static List<int> FilteredParRecordIds = new List<int>();
        public static List<int> FilteredFisRecordIds = new List<int>();
        public static List<int> FilteredTasksRecordIds = new List<int>();
        public static List<int> FilteredApprovalRecordIds = new List<int>();

        //public static AllItemsVM allItemsVM { get; set; }
        
    }
}