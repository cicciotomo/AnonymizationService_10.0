using System;

namespace AnonymizationService.Studies
{
    public class StudyMetadataDto
    {
        public Guid Id { get; set; }
        public string FhirResearchStudyId {get; set; }
        public string Acronym { get; set; }
        public string FullTitle { get; set; }
    }
}
