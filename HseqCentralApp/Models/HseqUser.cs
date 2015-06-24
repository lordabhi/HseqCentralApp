using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace HseqCentralApp.Models
{
    public class HseqUser
    {
        [Key]
        public int HseqUserID { get; set; }

        [Display(Name = "User")]
        public string UserID { get; set; }

        public virtual ApplicationUser User { get; set; }

        public Boolean Approver { get; set; }

        public Boolean Owner { get; set; }
        
        public Boolean Assignee { get; set; }
        
        public Boolean Coordinator { get; set; }

        public string FullName
        {
            get
            {
                return this.User.FirstName + " " + this.User.LastName;
            }

        }

    }
}
