using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public enum NcrState
    {
        New = 1,
        Pending = 2,

        [Display(Name = "Disposition Proposed")]
        DispositionProposed = 3,

        [Display(Name = "Disposition Approved")]
        DispositionApproved = 4,

        Closed = 5

    }
}