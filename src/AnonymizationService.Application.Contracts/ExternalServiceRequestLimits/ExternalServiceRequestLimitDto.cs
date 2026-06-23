using AnonymizationService.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ExternalServiceRequestLimits
{
    public class ExternalServiceRequestLimitDto : EntityDto<Guid>
    {
        public ExternalServiceRequestTypeEnum ExternalServiceRequestsType { get; set; }
        public int DailyNumberRequestsLimitInInterval { get; set; }
        public int NightlyNumberRequestsLimitInInterval { get; set; }
        public TimeSpan StartDailyRange { get; set; }
        public TimeSpan StartNightlyRange { get; set; }
        public int RequestsNumberIntervalMinutes { get; set; }
    }
}
