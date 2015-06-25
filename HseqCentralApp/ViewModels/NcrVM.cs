using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.ViewModels
{
    public class NcrVM
    {
        public Ncr Ncr { get; set; }

        //Approvals
        [Display(Name = "Disposition Approver")]
        public int? ApproverID { get; set; }
        public virtual HseqUser Approver { get; set; }
    }
}