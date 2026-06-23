using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter
{
    public class DocumentSectionExtractionResult
    {
        public string Id { get; set; }
        public string SectionName { get; set; }
        public List<Resource> FhirResources { get; set; }
        public List<TextAnalyticsEntity> Entities { get; set; }
        public List<TextAnalyticsRelation> Relations { get; set; }
    }
}
