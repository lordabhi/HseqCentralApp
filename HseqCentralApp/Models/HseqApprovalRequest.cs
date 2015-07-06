using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class HseqApprovalRequest : Delegatable
    {

        public HseqApprovalRequest() {

            Response = ApprovalResult.Waiting;
        }

        public HseqApprovalRequest(Delegatable delegatable) : base(delegatable) { }

        public ApprovalStatus Status { get; set; }

        public ApprovalResult Response { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Response Date")]
        public DateTime? ResponseDate { get; set; }

        [Display(Name = "Response Comment")]
        [DataType(DataType.MultilineText)]
        public String ResponseComment { get; set; }

    }
}