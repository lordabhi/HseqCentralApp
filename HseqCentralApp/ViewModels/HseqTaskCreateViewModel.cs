using HseqCentralApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.ViewModels
{
    public class HseqTaskCreateViewModel
    {

        [Display(Name = "Record Type")]
        public string RecordType { get; set; }

        [Display(Name = "Case No")]
        [DisplayFormat(DataFormatString = "{0:##-####}")]
        public string CaseNo { get; set; }

        [Display(Name = "Record No")]
        public string RecordNo { get; set; }

        [Display(Name = "Record Title")]
        public string RecordTitle { get; set; }

        [Display(Name = "Record Description")]
        public string RecordDescription { get; set; }

        [Display(Name = "Hseq Record")]
        public int? HseqRecordID { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select task status...")]
        public TaskStatus Status { get; set; }

        [Display(Name = "Owner")]
        public int OwnerID { get; set; }
        //public virtual HseqUser Owner { get; set; }

        [Display(Name = "Assignee")]
        public int AssigneeID { get; set; }
        //public virtual HseqUser Assignee { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Assigned")]
        public DateTime DateAssigned { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

    }
}