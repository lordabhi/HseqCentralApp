using HseqCentralApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.ViewModels
{
    public class ParCreateViewModel
    {

        [Display(Name = "Case No")]
        [DisplayFormat(DataFormatString = "{0:##-####}")]
        public string CaseNo { get; set; }

        [Display(Name = "Record No")]
        public string RecordNo { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }

        [Display(Name = "Job Number")]
        public String JobNumber { get; set; }

        [Display(Name = "Drawing Number")]
        public String DrawingNumber { get; set; }

        [Display(Name = "Record Type")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a record type")]
        public RecordType RecordType { get; set; }

        [Display(Name = "Entered By")]
        public String EnteredBy { get; set; }

        [Display(Name = "Reported By")]
        public String ReportedBy { get; set; }

        [Display(Name = "Created By")]
        public String CreatedBy { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Coordinator")]
        public int CoordinatorID { get; set; }

        public List<HseqUser> Coordinators { get; set; }

    }
}