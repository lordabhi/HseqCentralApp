using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class FisCode
    {
        public int Id { get; set; }
        public string CodeName { get; set; }
        public int FisCategoryId { get; set; }
        public virtual FisCategory FisCategory { get; set; }
    }
}