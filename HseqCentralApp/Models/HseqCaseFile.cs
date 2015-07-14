using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class HseqCaseFile
    {
        public HseqCaseFile()
        {

            this.HseqRecords = new HashSet<HseqRecord>();
        }

        [Key]
        public int HseqCaseFileID { get; set; }

        [Display(Name="Case No")]
        [DisplayFormat(DataFormatString = "{0:##-####}")]
        public string CaseNo { get; set; }

        [Display(Name = "Alfresco Node Reference")]
        public int AlfrescoNoderef { get; set; }

        public virtual ICollection<HseqRecord> HseqRecords { get; set; }
    }
}