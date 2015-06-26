using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.Services
{
    public class DelegatableService
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUser currentUser;

        public DelegatableService()
        {

            currentUser = GetCurrentUser();
        }

        public void AddHseqApprovalRequest(HseqRecord record, int? ApproverID, HseqApprovalRequest hseqApprovalRequest, ApplicationDbContext db)
        {
            if (record is Ncr)
            {
                Ncr ncr = (Ncr)record;

                if (ApproverID != null && ApproverID > 0)
                {

                    HseqApprovalRequest approvalRequest = new HseqApprovalRequest();
                    //approvalRequest.Owner = db.HseqUsers.Find(_RecordService.GetCurrentUser().Id);

                    approvalRequest.Owner = db.HseqUsers.Find(ApproverID);
                    approvalRequest.Assignee = db.HseqUsers.Find(GetCurrentApplicationUser().HseqUserID);
                    approvalRequest.DateAssigned = DateTime.Now;
                    approvalRequest.Title = hseqApprovalRequest.Title;
                    approvalRequest.Description = hseqApprovalRequest.Description;
                    if (hseqApprovalRequest.DueDate == null)
                    {
                        approvalRequest.DueDate = DateTime.Now.AddDays(14);
                    }
                    else {
                        approvalRequest.DueDate = hseqApprovalRequest.DueDate;
                    }
                    
                    approvalRequest.Status = ApprovalStatus.Active;
                    approvalRequest.Response = ApprovalResult.Waiting;

                    approvalRequest.HseqRecordID = ncr.HseqRecordID;
                    ncr.Delegatables.Add(approvalRequest);
                }
            }
        }

        public void AddHseqTaskRequest(HseqRecord record, int? ApproverID, HseqTask taskRequest, ApplicationDbContext db)
        {
            if (record is Ncr)
            {
                Ncr ncr = (Ncr)record;

                if (ApproverID != null && ApproverID > 0)
                {
                    //HseqTask taskRequest = new HseqTask();
                    taskRequest.Owner = db.HseqUsers.Find(ApproverID);
                    taskRequest.Assignee = db.HseqUsers.Find(GetCurrentApplicationUser().HseqUserID);
                    taskRequest.DateAssigned = DateTime.Now;
                    if (taskRequest.DueDate == null)
                    {
                        taskRequest.DueDate = DateTime.Now.AddDays(14);
                    }
                    else
                    {
                        taskRequest.DueDate = taskRequest.DueDate;
                    }
                    taskRequest.Status = TaskStatus.NotStarted;
                    taskRequest.HseqRecordID = ncr.HseqRecordID;
                    ncr.Delegatables.Add(taskRequest);
                }
            }
        }
        public ApplicationUser GetCurrentUser()
        {

            ApplicationUser currentUser = db.Users.Where(m => m.Email == HttpContext.Current.User.Identity.Name).First();
            return currentUser;

        }

        public HseqUser GetCurrentApplicationUser()
        {

            ApplicationUser currentUser = GetCurrentUser();
            HseqUser hseqUser = db.HseqUsers.Where(a => a.UserID == currentUser.Id).First();
            return hseqUser;
        }
    }
}