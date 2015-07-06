using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    //[TrackChanges]
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

        [Display(Name = "Disposition Type")]
        public int? DispositionTypeID { get; set; }

        public virtual DispositionType DispositionType { get; set; }

        //[Display(Name = "Disposition Approver")]
        //public string DispositionApproverID { get; set; }

        //public virtual ApproverDisposition DispositionApprover { get; set; }

        [Display(Name = "Disposition Notes")]
        public String DispositionNote { get; set; }

        [ForeignKey("DetectedInArea")]
        [Required(ErrorMessage = "Select Area Detected In")]
        [Display(Name = "Area Detected In")]
        public int DetectedInAreaID { get; set; }

        public virtual BusinessArea DetectedInArea { get; set; }

        [ForeignKey("ResponsibleArea")]
        //[Required(ErrorMessage = "Select Responsible Area")]
        [Display(Name = "Responsible Area")]
        public int? ResponsibleAreaID { get; set; }

        public virtual BusinessArea ResponsibleArea { get; set; }

        [DataType(DataType.MultilineText)]
        public String CauseDesc { get; set; }

        public String ResponsibleParty { get; set; }

    }

    public enum NcrSource
    {
        //[Description("Internal")]
        Internal = 1,

        External = 2,

        [Display(Name = "Customer Initiated")]
        CustomerInitiated = 3
    }

    public enum NcrState
    {
        New = 1,
        Pending = 2,

        [Display(Name = "Disposition Proposed")]
        DispositionProposed = 3,

        [Display(Name = "Disposition Approved")]
        DispositionApproved = 4,

        [Display(Name = "Disposition Rejected")]
        DispositionRejected = 5,

        Closed = 6
    }

}