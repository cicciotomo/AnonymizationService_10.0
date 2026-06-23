using AnonymizationService.Services.FhirService;
using AnonymizationService.Services.PlatformManagementConsole;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace AnonymizationService.HospitalPatients
{
    public class HospitalPatientService : DomainService, IHospitalPatientService
    {
        private readonly IHospitalPatientRepository _patientRepository;
        private readonly IPlatformManagementConsoleClient _platformManagementConsoleClient;

        public HospitalPatientService(IHospitalPatientRepository patientRepository, IFhirServiceClient fhirServiceClient, IPlatformManagementConsoleClient platformManagementConsoleClient)
        {
            _patientRepository = patientRepository;
            _platformManagementConsoleClient = platformManagementConsoleClient;
        }

        public async Task<HospitalPatient> CreatePatientAsync(string masterPatientIndex)
        {
            var patient = await _patientRepository.FindAsync(patient => patient.MasterPatientIndex == masterPatientIndex);
            if (patient is not null)
            {
                return patient;
            }

            var cloudPatientId = GuidGenerator.Create();

            return await _patientRepository.InsertAsync(new HospitalPatient(cloudPatientId, masterPatientIndex));
        }

        public async Task<HospitalPatient> UpdateLaboratoryLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            patient.LastLaboratoryDataUpdateDate = updateDate;
            return await _patientRepository.UpdateAsync(patient);
        }

        public async Task<HospitalPatient> UpdateClinicalLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            patient.LastClinicalDataUpdateDate = updateDate;
            return await _patientRepository.UpdateAsync(patient);
        }

        public async Task<HospitalPatient> UpdateDicomLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            patient.LastDicomDataUpdateDate = updateDate;
            return await _patientRepository.UpdateAsync(patient);
        }

		public async Task<HospitalPatient> UpdateDbUriLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate)
		{
			var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

			if (patient == null)
			{
				throw new Exception($"Unable to find patient with Id {cloudPatientId}");
			}

			patient.LastDbUriDataUpdateDate = updateDate;
			return await _patientRepository.UpdateAsync(patient);
		}

		public async Task<HospitalPatient> UpdatePathoxLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate)
		{
			var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

			if (patient == null)
			{
				throw new Exception($"Unable to find patient with Id {cloudPatientId}");
			}

			patient.LastPathoxDataUpdateDate = updateDate;
			return await _patientRepository.UpdateAsync(patient);
		}

        public async Task<HospitalPatient> UpdateRedCapLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            patient.LastRedcapDataUpdateDate = updateDate;
            return await _patientRepository.UpdateAsync(patient);
        }


        public async Task<HospitalPatient> UpdateMedicalDataLastUpdateDateForPatientAsync(Guid cloudPatientId, DateTime updateDate)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            patient.LastMedicalDataUpdateDate = updateDate;
            return await _patientRepository.UpdateAsync(patient);
        }

        public async Task<HospitalPatient> UpdateFhirIdForPatientAsync(Guid cloudPatientId, string fhirPatientId)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            patient.FhirPatientId = fhirPatientId;
            return await _patientRepository.UpdateAsync(patient);
        }

        public async Task IssuePatientDeletionRequest(Guid cloudPatientId)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            await _platformManagementConsoleClient.RequestPatientDeletionAsync(cloudPatientId);

            patient.RequireDeletion();
            await _patientRepository.UpdateAsync(patient);
        }

        public async Task DeletePatientAsync(Guid cloudPatientId)
        {
            var patient = await _patientRepository.GetAsync(p => p.CloudPatientId == cloudPatientId);

            if (patient == null)
            {
                throw new Exception($"Unable to find patient with Id {cloudPatientId}");
            }

            if (!patient.DeletionRequested)
            {
                throw new Exception("Patient deletion has not been requested yet.");
            }

            await _patientRepository.HardDeleteAsync(patient);
        }

    }
}
