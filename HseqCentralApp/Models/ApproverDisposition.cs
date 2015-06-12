using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class ApproverDisposition
    {
        [Key]
        public int ApproverDispositionID { get; set; }

        [Display(Name = "Disposition Approver")]
        public string ApproverID { get; set; }

        public virtual ApplicationUser Approver { get; set; }

        public string FullName
        {
            get
            {
                return this.Approver.FirstName + " " + this.Approver.LastName;
            }

        }
    
    }
}