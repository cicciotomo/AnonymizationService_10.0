using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class PatientListRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}
