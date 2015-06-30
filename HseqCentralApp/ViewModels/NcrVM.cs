using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.ViewModels
{
    public class NcrVM
    {

        public Ncr Ncr { get; set; }

        public int? HseqApprovalRequestID { get; set; }
        public virtual HseqApprovalRequest HseqApprovalRequest { get; set; }

        public IEnumerable<BusinessArea> DetectedInAreas { get; set; }
        public IEnumerable<BusinessArea> ResponsibleAreas { get; set; }
        public IEnumerable<DiscrepancyType> DiscrepancyTypes { get; set; }

        public IEnumerable<HseqUser> Coordinators { get; set; }

        public IEnumerable<DispositionType> DispositionTypes { get; set; }

        public IEnumerable<HseqUser> ApprovalOwners { get; set; }
        public IEnumerable<HseqUser> TaskOwners { get; set; }

        public bool ProposedDisposition { get; set; }

        public int? HseqTaskID { get; set; }
        public virtual HseqTask HseqTask { get; set; }

            //ViewBag.DispositionTypeID = new SelectList(db.DispositionTypes, "DispositionTypeID", "Name");


            //ViewBag.ApproverID = new SelectList(db.HseqUsers, "HseqUserID", "FullName");

        //[Display(Name = "Disposition Approver")]
        //public int? ApproverID { get; set; }
        //public virtual HseqUser Approver { get; set; }
    }
}