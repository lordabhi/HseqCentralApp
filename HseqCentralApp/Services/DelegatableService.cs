using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Helpers;
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

        //public void AddHseqApprovalRequest(HseqRecord record, int? ApproverID, HseqApprovalRequest hseqApprovalRequest, ApplicationDbContext db)
        //{
        //    if (record is Ncr)
        //    {
        //        Ncr ncr = (Ncr)record;

        //        if (hseqApprovalRequest == null) {
        //            hseqApprovalRequest = new HseqApprovalRequest();
        //        }

        //        if (ApproverID != null && ApproverID > 0)
        //        {

        //            HseqApprovalRequest approvalRequest = new HseqApprovalRequest();
        //            //approvalRequest.Owner = db.HseqUsers.Find(_RecordService.GetCurrentUser().Id);

        //            approvalRequest.Owner = db.HseqUsers.Find(ApproverID);
        //            approvalRequest.Assignee = db.HseqUsers.Find(GetCurrentApplicationUser().HseqUserID);
        //            approvalRequest.DateAssigned = DateTime.Now;
        //            if (hseqApprovalRequest.Title == null)
        //            {
        //                approvalRequest.Title = ncr.Title;
        //            }
        //            else {
        //                approvalRequest.Title = hseqApprovalRequest.Title;
        //            }

        //            if (hseqApprovalRequest.Description == null)
        //            {
        //                approvalRequest.Description = ncr.Description;
        //            }
        //            else {
        //                approvalRequest.Description = hseqApprovalRequest.Description;
        //            }
                    
        //            if (hseqApprovalRequest.DueDate == null || hseqApprovalRequest.DueDate <DateTime.Now.Subtract(TimeSpan.FromDays(1)))
        //            {
        //                approvalRequest.DueDate = DateTime.Now.AddDays(14);
        //            }
        //            else {
        //                approvalRequest.DueDate = hseqApprovalRequest.DueDate;
        //            }
                    
        //            approvalRequest.Status = ApprovalStatus.Active;
        //            approvalRequest.Response = ApprovalResult.Waiting;

        //            approvalRequest.HseqRecordID = ncr.HseqRecordID;
        //            ncr.Delegatables.Add(approvalRequest);
        //        }
        //    }
        //}

        public HseqApprovalRequest AddHseqApprovalRequest(HseqRecord record, HseqApprovalRequest approvalRequest, ApplicationDbContext db)
        {
           // if (record is Ncr)
            //{
                //Ncr ncr = (Ncr)record;

                if (approvalRequest == null)
                {
                    approvalRequest = new HseqApprovalRequest();
                }

                    approvalRequest.Owner = db.HseqUsers.Find(approvalRequest.OwnerID);

                    approvalRequest.DateAssigned = DateTime.Now;
                    
                    if (approvalRequest.DueDate == null || approvalRequest.DueDate < DateTime.Now.Subtract(TimeSpan.FromDays(1)))
                    {
                        approvalRequest.DueDate = DateTime.Now.AddDays(14);
                    }
                    else
                    {
                        approvalRequest.DueDate = approvalRequest.DueDate;
                    }

                    //approvalRequest.Status = ApprovalStatus.Active;
                    //approvalRequest.Response = ApprovalResult.Waiting;

                    //approvalRequest.HseqRecordID = ncr.HseqRecordID;

                    record.Delegatables.Add(approvalRequest);

                    return approvalRequest;

            //}

            //return null;
        }

        public void AddHseqTaskRequest(HseqRecord record, HseqTask taskRequest, ApplicationDbContext db)
        {
            //if (record is Ncr)
            //{
            //    Ncr ncr = (Ncr)record;

                    //HseqTask taskRequest = new HseqTask();

                    //taskRequest.Owner = db.HseqUsers.Find(ApproverID);

                    taskRequest.Owner = db.HseqUsers.Find(taskRequest.OwnerID);
                    taskRequest.Assignee = db.HseqUsers.Find(taskRequest.AssigneeID);
                    taskRequest.DateAssigned = DateTime.Now;
                    if (taskRequest.DueDate == null || taskRequest.DueDate < DateTime.Now.Subtract(TimeSpan.FromDays(1)))
                    {
                        taskRequest.DueDate = DateTime.Now.AddDays(14);
                    }
                    else
                    {
                        taskRequest.DueDate = taskRequest.DueDate;
                    }

                    if (taskRequest.Status == null)
                    {
                        taskRequest.Status = TaskStatus.NotStarted;
                    }
                    taskRequest.HseqRecordID = record.HseqRecordID;
                    record.Delegatables.Add(taskRequest);
            //}
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