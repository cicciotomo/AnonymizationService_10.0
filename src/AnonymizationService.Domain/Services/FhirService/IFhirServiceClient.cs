using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace AnonymizationService.Services.FhirService
{
    public interface IFhirServiceClient
    {
        Task CreateTransactionBundlesForResourcesAsync(List<Resource> resources);
        Task<Patient> UpsertPatientAsync(Guid cloudPatientId, AdministrativeGender gender, DateTime? birthDate);
        Task UpsertEncounterWithObservationsAsync(string fhirPatientId, List<FhirLaboratoryDataDto> laboratoryExams);
        Task<Organization> UpsertHospitalDepartment(string departmentName);
        Task<Encounter> UpsertEncounter(DateTime startDate, DateTime endDate, string patientIdentifier, string patientId, string identifier);
        Task<ResearchSubject> UpsertResearchSubjectAsync(string fhirPatientIdentifier, string fhirPatientId, string fhirStudyId, string fhirStudyIdentifier);
        Task<ResearchStudy> GetResearchStudyByIdAsync(string studyId);
		Task<Encounter> GetEncounterByIdentifier(string encounterIdentifier);
        Task<DocumentReference> UpsertDocumentReference(DocumentReference doc);

    }
}
