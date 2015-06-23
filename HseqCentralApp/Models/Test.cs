using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class Test : HseqRecord
    {

        [Key]
        public int Id { get; set; }

        public HseqRecord linkRecord { get; set; }

    }
}