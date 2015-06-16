using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class Fis : HseqRecord
    {

        public Fis() { }

        public Fis(HseqRecord hseqRecord) : base(hseqRecord) {}

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select a source")]
        [Display(Name = "Source")]
        public NcrSource NcrSource { get; set; }

        //public String Category { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Code is required")]
        public int? FisCodeID { get; set; }

        [Display(Name = "Code")]
        public virtual FisCode FisCode { get; set; }

    }
}