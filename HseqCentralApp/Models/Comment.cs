using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        public string Content { get; set; }

        [Display(Name = "Owner")]
        public int OwnerID { get; set; }
        public virtual HseqUser Owner { get; set; }

        [Display(Name = "TimeStamp")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateCreated { get; set; }

        //public string AssociatedType { get; set; }

        [Display(Name = "Hseq Record")]
        public int? HseqRecordID { get; set; }

        public virtual HseqRecord HseqRecord { get; set; }

        [Display(Name = "Delegatable Record")]
        public int? DelegatableID { get; set; }

        public virtual Delegatable Delegatable { get; set; }

        public CommentSource CommentSource { get; set; }
    }

    public enum CommentSource
    {
        //[Description("Internal")]
        Record = 1,

        Delegatable = 2
    }
}