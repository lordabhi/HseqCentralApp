using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public abstract class HseqRecord
    {
        public HseqRecord()
        {

            this.LinkedRecords = new HashSet<HseqRecord>();
            this.Delegatables = new HashSet<Delegatable>();
        }

        public HseqRecord(HseqRecord record)
        {
            this.AlfrescoNoderef = record.AlfrescoNoderef;
            this.Title = record.Title;
            this.Description = record.Description;
            this.HseqCaseFile = record.HseqCaseFile;
            this.HseqCaseFileID = record.HseqCaseFileID;
            this.HseqRecordID = record.HseqRecordID;

            this.EnteredBy = record.EnteredBy;
            this.ReportedBy = record.ReportedBy;
            //this.QualityCoordinator = record.QualityCoordinator;

        }


        [Key]
        public int HseqRecordID { get; set; }

        public int? AlfrescoNoderef { get; set; }

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

        [Display(Name = "Record Type")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a record type")]
        public RecordType RecordType { get; set; }

        [Display(Name = "Entered By")]
        public String EnteredBy { get; set; }

        [Display(Name = "Reported By")]
        public String ReportedBy { get; set; }

        [Display(Name = "Coordinator")]
        public int CoordinatorID { get; set; }
        public virtual HseqUser Coordinator { get; set; }

        public int? LinkedRecordsID { get; set; }
        public virtual ICollection<HseqRecord> LinkedRecords { get; set; }

        public int? HseqCaseFileID { get; set; }

        public virtual HseqCaseFile HseqCaseFile { get; set; }

        [Display(Name = "Job Number")]
        public String JobNumber { get; set; }

        [Display(Name = "Drawing Number")]
        public String DrawingNumber { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime? DateLastUpdated { get; set; }

        [Display(Name = "Created By")]
        public String  CreatedBy { get; set; }

        [Display(Name = "Last Updated By")]
        public String LastUpdatedBy { get; set; }

        //Approvals
        //[Display(Name = "Disposition Approver")]
        //public int? ApproverID { get; set; }
        //public virtual HseqUser Approver { get; set; }

        public virtual ICollection<Delegatable> Delegatables { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        /////////////////////////////////////////////////////

        public string DateCreatedForDisplay
        {
            get
            {
                return this.DateCreated.ToString("dd/MM/yyyy");
            }

        }

        public string LinkRecordForDisplay
        {
            get
            {
                return this.RecordType + ":"+this.RecordNo +" - "+ this.Title;
            }

        }

    }

    public enum RecordType
    {
        NCR = 1,
        FIS = 2,
        CAR = 3,
        PAR = 4
    }
}