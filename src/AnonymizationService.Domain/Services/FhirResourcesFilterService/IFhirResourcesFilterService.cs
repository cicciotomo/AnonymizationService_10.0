using AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace AnonymizationService.Services.FhirResourcesFilterService
{
    public interface IFhirResourcesFilterService
    {
        List<Resource> FilterFhirResources(List<DocumentSectionExtractionResult> resources, FilterManipulationData filterManipulationData);
    }
}