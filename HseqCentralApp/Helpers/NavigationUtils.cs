using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.Helpers
{
    public sealed class NavigationUtils
    {
        public static readonly string CHECKED_STATE_CHECKED = "Checked";
        public static readonly string CHECKED_STATE_UNCHECKED = "Unchecked";
        public static readonly string CHECKED_STATE_INDETERMINATE = "Indeterminate";

        private NavigationUtils() {}

        private static ApplicationDbContext db = new ApplicationDbContext();

        public static List<Ncr> GetFilteredNcrRecords()
        {
            IEnumerable<Ncr> ncrRecordsResponsibleArea = null;
            IEnumerable<Ncr> ncrRecordsCoordinators = null;

            List<Ncr> filteredNcrRecordsList = new List<Ncr>();

            bool ncrType = false;

            if (!String.IsNullOrEmpty(NavigationFilter.RecordTypeCheckState) && NavigationFilter.RecordTypeCheckState == CHECKED_STATE_INDETERMINATE)
            {
                if (NavigationFilter.RecordTypes != null)
                {

                    if (NavigationFilter.RecordTypes.Contains("NCR"))
                    {
                        ncrType = true;
                    }
                    else
                    {
                        return filteredNcrRecordsList;

                    }

                }
            }
           

            if (ncrType)
            {
                if (!String.IsNullOrEmpty(NavigationFilter.RecordTypeCheckState) && NavigationFilter.RecordTypeCheckState == CHECKED_STATE_INDETERMINATE)
                {
                    if (NavigationFilter.ResponsibleAreaIds != null)
                    {
                        ncrRecordsResponsibleArea = from ncr in db.NcrRecords
                                     //where ncr.ResponsibleAreaID != null
                                     //&& NavigationFilter.ResponsibleAreaIds.Contains(ncr.ResponsibleAreaID.Value)
                                     where NavigationFilter.ResponsibleAreaIds.Contains(ncr.ResponsibleAreaID.Value)
                                     select ncr;
                        
                    }
                }

                if (!String.IsNullOrEmpty(NavigationFilter.CoordinatorsCheckState) && NavigationFilter.CoordinatorsCheckState == CHECKED_STATE_INDETERMINATE)
                {
                    if (NavigationFilter.CoordinatorIds != null)
                    {
                        ncrRecordsCoordinators = from ncr in db.NcrRecords
                                     //where ncr.CoordinatorID != null
                                     //&& NavigationFilter.CoordinatorIds.Contains(ncr.CoordinatorID)
                                     where NavigationFilter.CoordinatorIds.Contains(ncr.CoordinatorID)
                                     select ncr;
                    }
                }

                if (ncrRecordsResponsibleArea != null && ncrRecordsCoordinators != null)
                {
                    return ncrRecordsResponsibleArea.Intersect(ncrRecordsCoordinators).ToList();
                }
                else if (ncrRecordsResponsibleArea != null)
                {
                    return ncrRecordsResponsibleArea.ToList();

                }
                else if (ncrRecordsCoordinators != null)
                {
                    return ncrRecordsCoordinators.ToList();
                }
            }
            
            return db.NcrRecords.ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////


        public static List<Car> GetFilteredCarRecords() {

            IEnumerable<Car> filteredCarRecords = null;
            List<Car> filteredCarRecordsList = new List<Car>();

            bool carType = false;
            bool resultsFound = false;

            if (!String.IsNullOrEmpty(NavigationFilter.RecordTypeCheckState)
                && (NavigationFilter.RecordTypeCheckState == CHECKED_STATE_INDETERMINATE) 
                && (NavigationFilter.RecordTypes != null))
            {

                if (NavigationFilter.RecordTypes.Contains("CAR"))
                {
                    carType = true;

                    if (carType)
                    {

                        if (!String.IsNullOrEmpty(NavigationFilter.CoordinatorsCheckState)
                            && (NavigationFilter.CoordinatorsCheckState == CHECKED_STATE_INDETERMINATE) 
                            && (NavigationFilter.CoordinatorIds != null))
                        {
                                filteredCarRecords = from record in db.CarRecords
                                                     where NavigationFilter.CoordinatorIds.Contains(record.CoordinatorID)
                                                     select record;

                                resultsFound = true;

                                if (filteredCarRecords != null)
                                {
                                    
                                    filteredCarRecordsList = filteredCarRecords.ToList();
                                }
                        }
                        else
                        {
                            resultsFound = true;
                            filteredCarRecordsList = db.CarRecords.ToList();
                        }

                    }
                }
            }
            else {

                resultsFound = true;
                filteredCarRecordsList = db.CarRecords.ToList();
            
            }

            return filteredCarRecordsList;
        }

        public static List<Par> GetFilteredParRecords()
        {

            IEnumerable<Par> filteredParRecords = null;
            List<Par> filteredParRecordsList = new List<Par>();

            bool ParType = false;
            bool resultsFound = false;

            if (!String.IsNullOrEmpty(NavigationFilter.RecordTypeCheckState)
                && (NavigationFilter.RecordTypeCheckState == "Indeterminate")
                && (NavigationFilter.RecordTypes != null))
            {

                if (NavigationFilter.RecordTypes.Contains("PAR"))
                {
                    ParType = true;

                    if (ParType)
                    {

                        if (!String.IsNullOrEmpty(NavigationFilter.CoordinatorsCheckState)
                            && (NavigationFilter.CoordinatorsCheckState == CHECKED_STATE_INDETERMINATE)
                            && (NavigationFilter.CoordinatorIds != null))
                        {
                            filteredParRecords = from record in db.ParRecords
                                                 where NavigationFilter.CoordinatorIds.Contains(record.CoordinatorID)
                                                 select record;

                            resultsFound = true;

                            if (filteredParRecords != null)
                            {

                                filteredParRecordsList = filteredParRecords.ToList();
                            }
                        }
                        else
                        {
                            resultsFound = true;
                            filteredParRecordsList = db.ParRecords.ToList();
                        }

                    }
                }
            }
            else
            {

                resultsFound = true;
                filteredParRecordsList = db.ParRecords.ToList();

            }

            return filteredParRecordsList;
        }

        public static List<Fis> GetFilteredFisRecords()
        {

            IEnumerable<Fis> filteredFisRecords = null;
            List<Fis> filteredFisRecordsList = new List<Fis>();

            bool FisType = false;
            bool resultsFound = false;

            if (!String.IsNullOrEmpty(NavigationFilter.RecordTypeCheckState)
                && (NavigationFilter.RecordTypeCheckState == CHECKED_STATE_INDETERMINATE)
                && (NavigationFilter.RecordTypes != null))
            {

                if (NavigationFilter.RecordTypes.Contains("FIS"))
                {
                    FisType = true;

                    if (FisType)
                    {

                        if (!String.IsNullOrEmpty(NavigationFilter.CoordinatorsCheckState)
                            && (NavigationFilter.CoordinatorsCheckState == CHECKED_STATE_INDETERMINATE)
                            && (NavigationFilter.CoordinatorIds != null))
                        {
                            filteredFisRecords = from record in db.FisRecords
                                                 where NavigationFilter.CoordinatorIds.Contains(record.CoordinatorID)
                                                 select record;

                            resultsFound = true;

                            if (filteredFisRecords != null)
                            {

                                filteredFisRecordsList = filteredFisRecords.ToList();
                            }
                        }
                        else
                        {
                            resultsFound = true;
                            filteredFisRecordsList = db.FisRecords.ToList();
                        }

                    }
                }
            }
            else
            {

                resultsFound = true;
                filteredFisRecordsList = db.FisRecords.ToList();

            }

            return filteredFisRecordsList;
        }

///////////////////////////////////////

        public static List<HseqTask> GetFilteredTasks()
        {

            IEnumerable<HseqTask> filteredTaskRecords = null;
            List<HseqTask> filteredTasksList = new List<HseqTask>();

            if (!String.IsNullOrEmpty(NavigationFilter.RecordTypeCheckState)
                && (NavigationFilter.RecordTypeCheckState == CHECKED_STATE_INDETERMINATE)
                && (NavigationFilter.RecordTypes != null))
                {

                if (NavigationFilter.RecordTypes.Contains("NCR"))
                {

                            filteredTaskRecords = from record in db.HseqTasks
                                        where NavigationFilter.FilteredNcrRecordIds.Contains(record.HseqRecordID.Value)
                                        select record;

                            filteredTasksList = filteredTasksList.Union(filteredTaskRecords).ToList();
                }
                if (NavigationFilter.RecordTypes.Contains("FIS"))
                {

                    filteredTaskRecords = from record in db.HseqTasks
                                    where NavigationFilter.FilteredFisRecordIds.Contains(record.HseqRecordID.Value)
                                    select record;

                    filteredTasksList = filteredTasksList.Union(filteredTaskRecords).ToList();
                }
                if (NavigationFilter.RecordTypes.Contains("CAR"))
                {

                    filteredTaskRecords = from record in db.HseqTasks
                                         where NavigationFilter.FilteredCarRecordIds.Contains(record.HseqRecordID.Value)
                                         select record;

                    filteredTasksList = filteredTasksList.Union(filteredTaskRecords).ToList();

                }
                if (NavigationFilter.RecordTypes.Contains("PAR"))
                {

                    filteredTaskRecords = from record in db.HseqTasks
                                         where NavigationFilter.FilteredParRecordIds.Contains(record.HseqRecordID.Value)
                                         select record;

                    filteredTasksList = filteredTasksList.Union(filteredTaskRecords).ToList();
                }
            }
            else
            {
                filteredTasksList = db.HseqTasks.ToList();

            }

            return filteredTasksList;
        }

        /////////////////////////////////////////////////////

        public static List<HseqApprovalRequest> GetFilteredApprovalRequests()
        {

            IEnumerable<HseqApprovalRequest> filteredApprovalRecords = null;
            List<HseqApprovalRequest> filteredApprovalRequestsList = new List<HseqApprovalRequest>();

            if (!String.IsNullOrEmpty(NavigationFilter.RecordTypeCheckState)
                && (NavigationFilter.RecordTypeCheckState == CHECKED_STATE_INDETERMINATE)
                && (NavigationFilter.RecordTypes != null))
            {

                if (NavigationFilter.RecordTypes.Contains("NCR"))
                {

                    filteredApprovalRecords = from record in db.HseqApprovalRequests
                                         where NavigationFilter.FilteredNcrRecordIds.Contains(record.HseqRecordID.Value)
                                         select record;

                    filteredApprovalRequestsList = filteredApprovalRequestsList.Union(filteredApprovalRecords).ToList();
                }
                if (NavigationFilter.RecordTypes.Contains("FIS"))
                {

                    filteredApprovalRecords = from record in db.HseqApprovalRequests
                                         where NavigationFilter.FilteredFisRecordIds.Contains(record.HseqRecordID.Value)
                                         select record;

                    filteredApprovalRequestsList = filteredApprovalRequestsList.Union(filteredApprovalRecords).ToList();
                }
                if (NavigationFilter.RecordTypes.Contains("CAR"))
                {

                    filteredApprovalRecords = from record in db.HseqApprovalRequests
                                         where NavigationFilter.FilteredCarRecordIds.Contains(record.HseqRecordID.Value)
                                         select record;

                    filteredApprovalRequestsList = filteredApprovalRequestsList.Union(filteredApprovalRecords).ToList();

                }
                if (NavigationFilter.RecordTypes.Contains("PAR"))
                {

                    filteredApprovalRecords = from record in db.HseqApprovalRequests
                                         where NavigationFilter.FilteredParRecordIds.Contains(record.HseqRecordID.Value)
                                         select record;

                    filteredApprovalRequestsList = filteredApprovalRequestsList.Union(filteredApprovalRecords).ToList();
                }
            }
            else
            {
                filteredApprovalRequestsList = db.HseqApprovalRequests.ToList();

            }

            return filteredApprovalRequestsList;
        }

        //////////////////////////////////////

        public static List<HseqRecord> GetFilteredAllItems()
        {

            IEnumerable<HseqRecord> filteredAllItemRecords = null;

            List<int> allRecordIds = NavigationFilter.FilteredNcrRecordIds
                .Union(NavigationFilter.FilteredCarRecordIds)
                .Union(NavigationFilter.FilteredParRecordIds)
                .Union(NavigationFilter.FilteredFisRecordIds)
                .Distinct()
                .ToList();

            filteredAllItemRecords = from record in db.HseqRecords
                                 where allRecordIds.Contains(record.HseqRecordID)
                                 select record;



            return filteredAllItemRecords.ToList();
        }


    }
}