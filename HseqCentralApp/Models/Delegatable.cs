using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public abstract class Delegatable
    {

        public Delegatable() {
            DateAssigned = DateTime.Today;
        }

        public Delegatable(Delegatable delegatable){}

        [Key]
        public int DelegatableID { get; set; }

        [ForeignKey("Owner")]
        [Display(Name = "Owner")]
        public int OwnerID { get; set; }
        public virtual HseqUser Owner { get; set; }

        [ForeignKey("Assignee")]
        [Display(Name = "Assignee")]
        public int AssigneeID { get; set; }
        public virtual HseqUser Assignee { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Assigned")]
        public DateTime DateAssigned{ get; set; }

        public String Title { get; set; }

        [DataType(DataType.MultilineText)]
        public String Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Hseq Record")]
        public int? HseqRecordID { get; set; }

        public virtual HseqRecord HseqRecord { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }

        public string DateAssignedForDisplay
        {
            get
            {
                return this.DateAssigned.ToString("dd/MM/yyyy");
            }

        }

        public string DueDateForDisplay
        {
            get
            {
                return this.DueDate.ToString("dd/MM/yyyy");
            }

        }

        
    }
}