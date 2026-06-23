using AnonymizationService.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class BatchUploadRequestDto : CreationAuditedEntityDto<Guid>
    {
        public PatientBatchUploadStatus Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
