using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{

    //Preventive Action record
    public class Par : HseqRecord
    {
    
        public Par() { }

        public Par(HseqRecord hseqRecord) : base(hseqRecord) { }

         public String status { get; set; }

    }
}