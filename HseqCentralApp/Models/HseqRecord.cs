using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public abstract class HseqRecord
    {
        public HseqRecord()
        {

            this.LinkedRecords = new HashSet<HseqRecord>();
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
            this.QualityCoordinator = record.QualityCoordinator;

            this.BusinessAreaID = record.BusinessAreaID;
        }


        [Key]
        public int HseqRecordID { get; set; }

        public int? AlfrescoNoderef { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        public String Description { get; set; }

        [Display(Name = "Record Type")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a record type")]
        public RecordType RecordType { get; set; }

        [Display(Name = "Entered By")]
        public String EnteredBy { get; set; }

        [Display(Name = "Reported By")]
        public String ReportedBy { get; set; }

        [Display(Name = "Quality Coordinator")]
        public String QualityCoordinator { get; set; }

        public virtual ICollection<HseqRecord> LinkedRecords { get; set; }

        public int? HseqCaseFileID { get; set; }

        public virtual HseqCaseFile HseqCaseFile { get; set; }

        [Display(Name = "Job Number")]
        public String JobNumber { get; set; }

        [Display(Name = "Drawing Number")]
        public String DrawingNumber { get; set; }

        [Required(ErrorMessage = "Select a business area")]
        [Display(Name = "Business Area")]
        public int BusinessAreaID { get; set; }

        public virtual BusinessArea BusinessArea { get; set; }
    }
}