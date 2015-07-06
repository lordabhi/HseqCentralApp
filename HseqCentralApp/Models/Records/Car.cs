using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    //Corrective Action record
    public class Car : HseqRecord
    {
         public Car() { }

         public Car(HseqRecord hseqRecord) : base(hseqRecord) { }

         public String status { get; set; }

    }
}