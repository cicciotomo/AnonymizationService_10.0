using System;

namespace AnonymizationService.Services.PlatformManagementConsole;

public class StudyMetadata
{
    public Guid Id { get; set; }
    public string FhirResearchStudyId {get; set; }
    public string Acronym { get; set; }
    public string FullTitle { get; set; }
}