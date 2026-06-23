using AnonymizationService.PatientUploadRequests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.HospitalPatients
{
    public interface IHospitalPatientRepository : IRepository<HospitalPatient>
    {
        Task<List<HospitalPatient>> GetListAsync(int skipCount, int maxResultCount, string sorting, string filter = null);
        Task<List<HospitalPatient>> GetFilteredtListAsync(int skipCount, int maxResultCount, string sorting, DateTime? creationTime, Guid? CloudPatientId, string MasterPatientIndex, string FhirPatientId);
    }
}
