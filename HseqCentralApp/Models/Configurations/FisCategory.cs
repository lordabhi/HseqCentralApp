using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class FisCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<FisCode> FisCodes { get; set; }
    }
}