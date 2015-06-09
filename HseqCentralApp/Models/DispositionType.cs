using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class DispositionType
    {
        [Key]
        public int DispositionTypeID { get; set; }

        [Display(Name = "Disposition Type")]
        public String Name { get; set; }
    }
}