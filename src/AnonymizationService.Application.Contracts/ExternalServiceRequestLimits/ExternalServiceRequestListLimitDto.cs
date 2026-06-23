using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ExternalServiceRequestLimits
{
    public class ExternalServiceRequestListLimitDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
