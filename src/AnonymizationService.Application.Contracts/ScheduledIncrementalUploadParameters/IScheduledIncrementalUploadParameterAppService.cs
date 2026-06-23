using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ScheduledIncrementalUploadParameters
{
    public interface IScheduledIncrementalUploadParameterAppService
    {
        Task<ScheduledIncrementalUploadParameterDto> GetAsync(Guid id);
        Task<PagedResultDto<ScheduledIncrementalUploadParameterDto>> GetListAsync(ScheduledIncrementalUploadParameterListDto scheduledIncrementalUploadParameterListDto);
        Task<ScheduledIncrementalUploadParameterDto> UpdateAsync(ScheduledIncrementalUploadParameterDto scheduledIncrementalUploadParameterDto);
    }
}
