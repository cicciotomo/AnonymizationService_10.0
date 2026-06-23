using System;
using System.Threading.Tasks;

namespace AnonymizationService.HospitalPatients;

public interface IHospitalPatientService
{
    Task<HospitalPatient> CreatePatientAsync(string masterPatientIndex);
    Task<HospitalPatient> UpdateLaboratoryLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate);
    Task<HospitalPatient> UpdateClinicalLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate);
    Task<HospitalPatient> UpdateDicomLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate);
	Task<HospitalPatient> UpdateDbUriLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate);
	Task<HospitalPatient> UpdateMedicalDataLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate);
    Task<HospitalPatient> UpdateFhirIdForPatientAsync(Guid cloudPatientId, string fhirPatientId);
    Task DeletePatientAsync(Guid cloudPatientId);
    Task IssuePatientDeletionRequest(Guid cloudPatientId);
	Task<HospitalPatient> UpdatePathoxLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate);
    Task<HospitalPatient> UpdateRedCapLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate);

}