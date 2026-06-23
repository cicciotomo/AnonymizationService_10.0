using System;

namespace AnonymizationService.Jobs.UpsertFhirLaboratoryData
{
    public class FhirLaboratoryExamResultDto
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string ExamType { get; set; }
        public string ExamTypeDescription { get; set; }
        public string ReferenceLow { get; set; }
        public string ReferenceHigh { get; set; }
        public string ReferenceRange { get; set; }
        public string Comment { get; set; }
        public string Unit { get; set; }
        public DateTime? ResultDate { get; set; }
        public string ExamMethod { get; set; }
        public string ExamMethodDescription { get; set; }
    }
}
