using System.Collections.Generic;

namespace AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter
{
    public interface ITextAnalyticsResponseToFhirDataAdapter
    {
        List<DocumentSectionExtractionResult> ExtractFhirResources(List<DocumentSection> documentSections);
    }
}