using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.ViewModels
{
    public class HseqTaskVM
    {
        public HseqTaskVM() {
            //HseqTasks = (List<HseqTask>)Ncr.Delegatables.OfType<HseqTask>().ToList();
        }

        public HseqTaskVM(Ncr ncr)
        {
            this.HseqRecord = ncr;
            HseqTasks = (List<HseqTask>)this.HseqRecord.Delegatables.OfType<HseqTask>().ToList();
        }

        public HseqRecord HseqRecord { get; set; }
       
        public int? ApproverID { get; set; }
        public virtual HseqUser Approver { get; set; }

        public HseqTask HseqTask { get; set; }

        public ICollection<HseqTask> HseqTasks { get; set; }

        public ICollection<HseqTask> OwnedTasks { get; set; }
        public ICollection<HseqTask> AssignedTasks { get; set; }
    }
}