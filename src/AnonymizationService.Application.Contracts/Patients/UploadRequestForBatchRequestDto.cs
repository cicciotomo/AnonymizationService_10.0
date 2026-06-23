using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class UploadRequestForBatchRequestDto : PagedAndSortedResultRequestDto
    {
        [Required]
        public Guid BatchId { get; set; }
    }
}
