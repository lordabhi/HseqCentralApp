using HseqCentralApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.ViewModels
{
    public class CarCreateViewModel
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

        //////////////////////////////////////////////////////////////
       // [Key]
       // public int HseqRecordID { get; set; }

       // public int? AlfrescoNoderef { get; set; }

       // public virtual HseqUser Coordinator { get; set; }

       // public int? LinkedRecordsID { get; set; }
       // public virtual ICollection<HseqRecord> LinkedRecords { get; set; }

       // public int? HseqCaseFileID { get; set; }

       // public virtual HseqCaseFile HseqCaseFile { get; set; }

       // [Display(Name = "Last Updated")]
       // public DateTime? DateLastUpdated { get; set; }

       // [Display(Name = "Last Updated By")]
       // public String LastUpdatedBy { get; set; }


       // public virtual ICollection<Delegatable> Delegatables { get; set; }

       // public virtual ICollection<Comment> Comments { get; set; }

       //// public String status { get; set; }

    }
}