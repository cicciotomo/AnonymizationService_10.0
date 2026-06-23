using AnonymizationService.Permissions;
using AnonymizationService.Services.ThrottlingManager;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ExternalServiceRequestLimits
{
    [Authorize(AnonymizationServicePermissions.ExternalServiceRequestLimitManagementPermission)]

    public class ExternalServiceRequestLimitAppService : AnonymizationServiceAppService, IExternalServiceRequestLimitAppService
    {
        private readonly IExternalServiceRequestLimitRepository _externalServiceRequestLimitRepository;
        private readonly IThrottlingManager _throttlingManager;

        public ExternalServiceRequestLimitAppService(IExternalServiceRequestLimitRepository externalServiceRequestLimitRepository, IThrottlingManager throttlingManager)
        {
            _externalServiceRequestLimitRepository = externalServiceRequestLimitRepository;
            _throttlingManager = throttlingManager;
        }

        public async Task<ExternalServiceRequestLimitDto> GetAsync(Guid id)
        {
            var existingExternalServiceRequestLimit = await _externalServiceRequestLimitRepository.GetAsync(id);
            return ObjectMapper.Map<ExternalServiceRequestLimit, ExternalServiceRequestLimitDto>(existingExternalServiceRequestLimit);
        }

        public async Task<PagedResultDto<ExternalServiceRequestLimitDto>> GetListAsync(ExternalServiceRequestListLimitDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(ExternalServiceRequestLimit.ExternalServiceRequestsType);
            }

            var existingExternalServiceRequestLimits = await _externalServiceRequestLimitRepository.GetListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.Filter
            );
            var totalCount = existingExternalServiceRequestLimits.Count;

            var externalServiceRequestLimitDto = ObjectMapper.Map<List<ExternalServiceRequestLimit>, List<ExternalServiceRequestLimitDto>>(existingExternalServiceRequestLimits);

            return new PagedResultDto<ExternalServiceRequestLimitDto>(totalCount, externalServiceRequestLimitDto);
        }

        public async Task<ExternalServiceRequestLimitDto> UpdateAsync(UpdateExternalServiceRequestLimitDto updateExternalServiceRequestLimitDto)
        {
            var existingExternalServiceRequestLimit = await _externalServiceRequestLimitRepository.GetAsync(updateExternalServiceRequestLimitDto.Id);

            existingExternalServiceRequestLimit.DailyNumberRequestsLimitInInterval = updateExternalServiceRequestLimitDto.DailyNumberRequestsLimitInInterval;
            existingExternalServiceRequestLimit.NightlyNumberRequestsLimitInInterval = updateExternalServiceRequestLimitDto.NightlyNumberRequestsLimitInInterval;
            existingExternalServiceRequestLimit.StartDailyRange = updateExternalServiceRequestLimitDto.StartDailyRange;
            existingExternalServiceRequestLimit.StartNightlyRange = updateExternalServiceRequestLimitDto.StartNightlyRange;
            existingExternalServiceRequestLimit.RequestsNumberIntervalMinutes = updateExternalServiceRequestLimitDto.RequestsNumberIntervalMinutes;

            var updatedExternalServiceRequestLimit = await _externalServiceRequestLimitRepository.UpdateAsync(existingExternalServiceRequestLimit);
            await _throttlingManager.RetrieveExternalServiceRequestLimits();
            return ObjectMapper.Map<ExternalServiceRequestLimit, ExternalServiceRequestLimitDto>(updatedExternalServiceRequestLimit);
        }
    }
}
