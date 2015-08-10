using HseqCentralApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HseqCentralApp.ViewModels
{
    public class CarVM
    {

        public Car Car { get; set; }
        public bool LinkedRecord { get; set; }

        public HseqRecord SourceRecord { get; set; }
        public int SourceRecordId { get; set; }
    }
}