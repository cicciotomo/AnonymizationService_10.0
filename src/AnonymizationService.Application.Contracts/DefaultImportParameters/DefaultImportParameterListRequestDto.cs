using Volo.Abp.Application.Dtos;

namespace AnonymizationService.DefaultImportParameters
{
    public class DefaultImportParameterListRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
