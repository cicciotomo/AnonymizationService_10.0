using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class UploadRequestForPatientRequestDto : PagedAndSortedResultRequestDto
    {
        [Required]
        public string MasterPatientIndex { get; set; }
    }
}
