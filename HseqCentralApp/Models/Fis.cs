using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class Fis : HseqRecord
    {

        public Fis() { }

        public Fis(HseqRecord hseqRecord)
            : base(hseqRecord)
        {

        }

        public String Category { get; set; }
    }
}