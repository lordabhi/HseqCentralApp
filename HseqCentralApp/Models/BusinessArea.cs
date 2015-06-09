using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class BusinessArea
    {

        [Key]
        public int BusinessAreaID { get; set; }

        [Display(Name = "Business Area")]
        public String Name { get; set; }
    }
}