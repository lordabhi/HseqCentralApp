using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.ViewModels
{
    public class HseqApprovalRequestVM
    {
        public ICollection<HseqApprovalRequest> OwnedRequests { get; set; }
        public ICollection<HseqApprovalRequest> AssignedRequests { get; set; }
    }
}