using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.Services
{
    public class ApprovalService
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUser currentUser;

        public ApprovalService()
        {

            currentUser = GetCurrentUser();
        }

        public void AddHseqApprovalRequest(HseqRecord record, int? ApproverID, ApplicationDbContext db)
        {
            if (record is Ncr)
            {
                Ncr ncr = (Ncr)record;

                if (ncr.ApproverID != null && ncr.ApproverID > 0)
                {

                    HseqApprovalRequest approvalRequest = new HseqApprovalRequest();
                    //approvalRequest.Owner = db.HseqUsers.Find(_RecordService.GetCurrentUser().Id);

                    approvalRequest.Owner = db.HseqUsers.Find(ncr.ApproverID);
                    approvalRequest.Assignee = db.HseqUsers.Find(ApproverID);
                    approvalRequest.DateAssigned = DateTime.Now;
                    approvalRequest.Title = ncr.Title;
                    approvalRequest.Description = ncr.Description;
                    approvalRequest.DueDate = DateTime.Now.AddDays(14);
                    approvalRequest.Status = ApprovalStatus.Active;
                    approvalRequest.Response = ApprovalResult.Waiting;

                    approvalRequest.HseqRecordID = ncr.HseqRecordID;
                    ncr.Approvals.Add(approvalRequest);
                }
            }
        }

        public ApplicationUser GetCurrentUser()
        {

            ApplicationUser currentUser = db.Users.Where(m => m.Email == HttpContext.Current.User.Identity.Name).First();
            return currentUser;

        }
    }
}