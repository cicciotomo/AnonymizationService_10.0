using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ExternalServiceRequestLimits
{
    public interface IExternalServiceRequestLimitAppService
    {
        Task<ExternalServiceRequestLimitDto> GetAsync(Guid id);
        Task<PagedResultDto<ExternalServiceRequestLimitDto>> GetListAsync(ExternalServiceRequestListLimitDto requestDto);
        Task<ExternalServiceRequestLimitDto> UpdateAsync(UpdateExternalServiceRequestLimitDto updateExternalServiceRequestLimitDto);
    }
}
