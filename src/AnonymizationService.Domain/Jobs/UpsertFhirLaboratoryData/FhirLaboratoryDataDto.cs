using System;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.UpsertFhirLaboratoryData
{
    public class FhirLaboratoryDataDto
    {
        public string Id { get; set; }
        public DateTime ExamStartDate { get; set; }
        public DateTime ExamEndDate { get; set; }
        public List<FhirLaboratoryExamResultDto> ValueResults { get; set; }
        public Guid CloudPatientId { get; set; }
        public DateTime? CloudUploadDate { get; set; }
    }
}
