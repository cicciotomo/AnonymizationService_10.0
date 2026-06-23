using AnonymizationService.Enums;
using AnonymizationService.ExternalServiceRequestLimits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.Services.ThrottlingManager
{
    public class ThrottlingManager : IThrottlingManager, ISingletonDependency
    {
        private readonly ILogger<ThrottlingManager> _logger;

        private readonly IRepository<ExternalServiceRequestLimit, Guid> _externalServiceRequestLimitRepository;
        public List<ExternalServiceRequestLimit> externalServiceRequestLimits { get; set; }

        public ThrottlingManager(IRepository<ExternalServiceRequestLimit, Guid> externalServiceRequestLimitRepository, ILogger<ThrottlingManager> logger)
        {
            _externalServiceRequestLimitRepository = externalServiceRequestLimitRepository;
            _logger = logger;
        }

        public async Task RetrieveExternalServiceRequestLimits()
        {
            externalServiceRequestLimits = new List<ExternalServiceRequestLimit>();

            var dbExternalServiceRequestLimits = await _externalServiceRequestLimitRepository.GetListAsync();

            foreach (var dbExternalServiceRequestLimit in dbExternalServiceRequestLimits)
            {
                externalServiceRequestLimits.Add(new ExternalServiceRequestLimit(
                  dbExternalServiceRequestLimit.ExternalServiceRequestsType
                , dbExternalServiceRequestLimit.DailyNumberRequestsLimitInInterval
                , dbExternalServiceRequestLimit.NightlyNumberRequestsLimitInInterval
                , dbExternalServiceRequestLimit.StartDailyRange
                , dbExternalServiceRequestLimit.StartNightlyRange
                , dbExternalServiceRequestLimit.RequestsNumberIntervalMinutes));
            }
        }

        public async Task SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum externalServiceRequestLimitEnum)
        {
            var count = 1;
            while (!TryAddRequest(externalServiceRequestLimitEnum))
            {
                _logger.LogInformation("Waiting for wait time to be over for service: {service}", externalServiceRequestLimitEnum.ToString());

                await Task.Delay(10 * count);
                count++;
            }
        }

        private bool TryAddRequest(ExternalServiceRequestTypeEnum externalServiceRequestLimitEnum)
        {
            var externalServiceRequestLimit = externalServiceRequestLimits.Where(o => o.ExternalServiceRequestsType == externalServiceRequestLimitEnum).FirstOrDefault();

            if (externalServiceRequestLimit.NewRequestCanBeExecuted())
            {
                externalServiceRequestLimit.AddNewRequest();
                return true;
            }
            return false;
        }
    }
}
