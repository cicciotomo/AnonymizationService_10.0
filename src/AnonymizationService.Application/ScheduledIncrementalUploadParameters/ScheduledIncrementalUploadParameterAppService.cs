using AnonymizationService.Permissions;
using AnonymizationService.ScheduledUploadParameters;
using AnonymizationService.Services.UploadIncrementalDataManager;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ScheduledIncrementalUploadParameters
{
    [Authorize(AnonymizationServicePermissions.ScheduledIncrementalUploadParameterManagementPermission)]

    public class ScheduledIncrementalUploadParameterAppService : AnonymizationServiceAppService, IScheduledIncrementalUploadParameterAppService
    {
        private readonly IScheduledIncrementalUploadParameterRepository _scheduledIncrementalUploadParameterRepository;
        private readonly IUploadIncrementalDataManager _uploadIncrementalDataManager;

        public ScheduledIncrementalUploadParameterAppService(IScheduledIncrementalUploadParameterRepository scheduledIncrementalUploadParameterRepository,
            IUploadIncrementalDataManager uploadIncrementalDataManager)
        {
            _scheduledIncrementalUploadParameterRepository = scheduledIncrementalUploadParameterRepository;
            _uploadIncrementalDataManager = uploadIncrementalDataManager;
        }
        public async Task<ScheduledIncrementalUploadParameterDto> GetAsync(Guid id)
        {
            var scheduledIncrementalUploadParameter = await _scheduledIncrementalUploadParameterRepository.GetAsync(id);
            return ObjectMapper.Map<ScheduledIncrementalUploadParameter, ScheduledIncrementalUploadParameterDto>(scheduledIncrementalUploadParameter);
        }

        public async Task<PagedResultDto<ScheduledIncrementalUploadParameterDto>> GetListAsync(ScheduledIncrementalUploadParameterListDto scheduledIncrementalUploadParameterListDto)
        {

            if (scheduledIncrementalUploadParameterListDto.Sorting.IsNullOrWhiteSpace())
            {
                scheduledIncrementalUploadParameterListDto.Sorting = nameof(ScheduledIncrementalUploadParameter.ParameterKey);
            }

            var existingScheduledIncrementalUploadParameters = await _scheduledIncrementalUploadParameterRepository.GetListAsync(
                scheduledIncrementalUploadParameterListDto.SkipCount,
                scheduledIncrementalUploadParameterListDto.MaxResultCount,
                scheduledIncrementalUploadParameterListDto.Sorting,
                scheduledIncrementalUploadParameterListDto.Filter
            );
            var totalCount = existingScheduledIncrementalUploadParameters.Count;

            var scheduledIncrementalUploadParameterDtoListDto = ObjectMapper.Map<List<ScheduledIncrementalUploadParameter>, List<ScheduledIncrementalUploadParameterDto>>(existingScheduledIncrementalUploadParameters);

            return new PagedResultDto<ScheduledIncrementalUploadParameterDto>(totalCount, scheduledIncrementalUploadParameterDtoListDto);
        }

        public async Task<ScheduledIncrementalUploadParameterDto> UpdateAsync(ScheduledIncrementalUploadParameterDto scheduledIncrementalUploadParameterDto)
        {
            var existingScheduledIncrementalUploadParameter = await _scheduledIncrementalUploadParameterRepository.GetAsync(scheduledIncrementalUploadParameterDto.Id);

            existingScheduledIncrementalUploadParameter.Value = scheduledIncrementalUploadParameterDto.Value;

            var updatedScheduledIncrementalUploadParameter = await _scheduledIncrementalUploadParameterRepository.UpdateAsync(existingScheduledIncrementalUploadParameter);

            await _uploadIncrementalDataManager.Initialize();

            return ObjectMapper.Map<ScheduledIncrementalUploadParameter, ScheduledIncrementalUploadParameterDto>(updatedScheduledIncrementalUploadParameter);
        }
    }
}
