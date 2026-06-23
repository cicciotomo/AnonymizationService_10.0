using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ScheduledIncrementalUploadParameters
{

    public class ScheduledIncrementalUploadParameterListDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
