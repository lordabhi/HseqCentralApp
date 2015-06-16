using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class Ncr: HseqRecord
    {
        public Ncr(){}

        public Ncr(HseqRecord hseqRecord): base(hseqRecord) {}

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a source")]
        [Display(Name = "Source")]
        public NcrSource NcrSource { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a state")]
        [Display(Name = "State")]
        public NcrState NcrState { get; set; }

        [Required(ErrorMessage = "Select a discrepancy type")]
        [Display(Name = "Discrepancy Type")]
        public int DiscrepancyTypeID { get; set; }

        public virtual DiscrepancyType DiscrepancyType { get; set; }


        //[Required(ErrorMessage = "Select a business area")]
        //[Display(Name="Business Area")]
        //public int BusinessAreaID { get; set; }

        //public virtual BusinessArea BusinessArea { get; set; }

        [Display(Name = "Disposition Type")]
        public int? DispositionTypeID { get; set; }

        public virtual DispositionType DispositionType { get; set; }

        [Display(Name = "Disposition Approver")]
        public string DispositionApproverID { get; set; }

        public virtual ApproverDisposition DispositionApprover { get; set; }

        [Display(Name = "Disposition Notes")]
        public String DispositionNote { get; set; }


    }
}