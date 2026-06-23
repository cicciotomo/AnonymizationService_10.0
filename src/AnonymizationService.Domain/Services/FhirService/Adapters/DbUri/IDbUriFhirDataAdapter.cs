using AnonymizationService.Jobs.UpsertFhirDbUriData;
using AnonymizationService.Services.DbUri;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using Patient = AnonymizationService.Services.DbUri.Patient;

namespace AnonymizationService.Services.FhirService.Adapters.DbUri
{
    public interface IDbUriFhirDataAdapter
    {
        List<Resource> Transform(List<Event> events, List<Item> fupItems, DbUriTransformationAdditionalInfo dbUriTransformationAdditionalInfo);
        (string encounterIdentifier, List<Resource> resources) TransformFupEncounter(FupData fupData, DbUriTransformationAdditionalInfo dbUriTransformationAdditionalInfo);
        (string encounterIdentifier, List<Resource> resources) TransformPatientEncounter(Patient patient, DbUriTransformationAdditionalInfo dbUriTransformationAdditionalInfo);
    }
}
