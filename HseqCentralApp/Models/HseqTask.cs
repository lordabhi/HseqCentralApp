using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HseqCentralApp.Models
{
    public class HseqTask : Delegatable
    {

        public HseqTask(){}

        public HseqTask(Delegatable delegatable) : base(delegatable) { }

        [Range(1, int.MaxValue, ErrorMessage = "Select task status...")]
        public TaskStatus Status { get; set; }

    }

    public enum TaskStatus
    {
        [Description("Not Started")]
        [Display(Name = "Not Started")]
        NotStarted = 1,
        Active = 2,
        Completed = 3,
        Canceled = 4
    }
}