using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.ViewModels
{
    public static class AllItemsVM
    {
        public static List<Ncr> NcrRecords { get; set; }
        public static List<Car> CarRecords { get; set; }
        public static List<Par> ParRecords { get; set; }
        public static List<Fis> FisRecords { get; set; }
        public static List<HseqTask> TaskRecords { get; set; }
        public static List<HseqApprovalRequest> ApprovalRecords { get; set; }

    }
}