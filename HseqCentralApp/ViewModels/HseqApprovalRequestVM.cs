using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.ViewModels
{
    public class HseqApprovalRequestVM
    {

        public HseqApprovalRequestVM() {}

        public HseqApprovalRequestVM(Ncr ncr)
        {
            this.Ncr = ncr;
            HseqApprovalRequests = (List<HseqApprovalRequest>)this.Ncr.Delegatables.OfType<HseqApprovalRequest>().ToList();
        }

        public Ncr Ncr { get; set; }
       
        public int? ApproverID { get; set; }
        public virtual HseqUser Approver { get; set; }

        public HseqApprovalRequest HseqApprovalRequest { get; set; }

        public ICollection<HseqApprovalRequest> HseqApprovalRequests { get; set; }

        public ICollection<HseqApprovalRequest> OwnedRequests { get; set; }
        public ICollection<HseqApprovalRequest> AssignedRequests { get; set; }
    }
}