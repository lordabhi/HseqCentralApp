using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.ViewModels
{
    public class DelegatableVM
    {

        public HseqRecord HseqRecord { get; set; }

        public int? HseqApprovalRequestID { get; set; }
        public virtual HseqApprovalRequest HseqApprovalRequest { get; set; }


        public IEnumerable<HseqUser> ApprovalOwners { get; set; }
        public IEnumerable<HseqUser> TaskOwners { get; set; }

        public bool ProposedDisposition { get; set; }

        public int? HseqTaskID { get; set; }
        public virtual HseqTask HseqTask { get; set; }

    }
}