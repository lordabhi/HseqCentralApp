using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public enum NcrSource
    {
        [Description("Internal")]
        Internal = 1,

        External = 2,

        [Display(Name="Customer Initiated")]
        CustomerInitiated = 3
    }

}