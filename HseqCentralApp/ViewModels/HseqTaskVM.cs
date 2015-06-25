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
            HseqTasks = (ICollection<HseqTask>)Ncr.Delegatables.OfType<HseqTask>().ToList();
        }

        public HseqTaskVM(Ncr ncr)
        {
            this.Ncr = ncr;
            HseqTasks = (List<HseqTask>)this.Ncr.Delegatables.OfType<HseqTask>().ToList();
        }

        public Ncr Ncr { get; set; }
       
        public int? ApproverID { get; set; }
        public virtual HseqUser Approver { get; set; }

        public HseqTask HseqTask { get; set; }

        public ICollection<HseqTask> HseqTasks { get; set; }
    }
}