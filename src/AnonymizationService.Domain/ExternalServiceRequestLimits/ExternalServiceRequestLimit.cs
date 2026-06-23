using AnonymizationService.Enums;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.ExternalServiceRequestLimits
{

    public class ExternalServiceRequestLimit : Entity<Guid>
    {
        public ExternalServiceRequestTypeEnum ExternalServiceRequestsType { get; set; }
        public int DailyNumberRequestsLimitInInterval { get; set; }
        public int NightlyNumberRequestsLimitInInterval { get; set; }
        public TimeSpan StartDailyRange { get; set; }
        public TimeSpan StartNightlyRange { get; set; }
        public int RequestsNumberIntervalMinutes { get; set; }

        private List<DateTime> Requests { get; set; }

        protected ExternalServiceRequestLimit() { }

        public ExternalServiceRequestLimit(
            ExternalServiceRequestTypeEnum externalServiceRequestLimit
            , int dailyNumberRequestsLimitInInterval
            , int nightlyNumberRequestsLimitInInterval
            , TimeSpan startDailyRange
            , TimeSpan startNightlyRange
            , int requestsNumberIntervalMinutes
            )
        {
            ExternalServiceRequestsType = externalServiceRequestLimit;
            DailyNumberRequestsLimitInInterval = dailyNumberRequestsLimitInInterval;
            NightlyNumberRequestsLimitInInterval = nightlyNumberRequestsLimitInInterval;
            StartDailyRange = startDailyRange;
            StartNightlyRange = startNightlyRange;
            Requests = new List<DateTime>();
            RequestsNumberIntervalMinutes = requestsNumberIntervalMinutes;
        }



        public bool NewRequestCanBeExecuted()
        {
            Requests.RemoveAll(d => d < DateTime.UtcNow.AddMinutes(-RequestsNumberIntervalMinutes));

            if (Requests.Count <= GetCurrentNumberRequestsLimit())
            {
                return true;
            }
            return false;
        }

        private int GetCurrentNumberRequestsLimit()
        {
            if (DateTime.UtcNow.TimeOfDay >= StartDailyRange && DateTime.UtcNow.TimeOfDay < StartNightlyRange)
            {
                return DailyNumberRequestsLimitInInterval;
            }
            return NightlyNumberRequestsLimitInInterval;
        }
        public void AddNewRequest()
        {
            Requests.Add(DateTime.UtcNow);
        }

    }
}
