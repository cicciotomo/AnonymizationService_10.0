using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.DefaultImportParameters
{
    public interface IDefaultImportParameterAppService
    {
        Task<DefaultImportParameterDto> GetAsync(Guid id);
        Task<PagedResultDto<DefaultImportParameterDto>> GetListAsync(DefaultImportParameterListRequestDto requestDto);
        Task<DefaultImportParameterDto> UpdateAsync(UpdateDefaultImportParameterDto updateDefaultImportParameterDto);
    }
}
