using AnonymizationService.EntityFrameworkCore;
using AnonymizationService.PatientUploadRequests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.HospitalPatients
{
    public class EfCoreHospitalPatientRepository :
        EfCoreRepository<AnonymizationServiceDbContext, HospitalPatient>,
        IHospitalPatientRepository
    {
        public EfCoreHospitalPatientRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<HospitalPatient>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting,
            string filter = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .WhereIf(
                    !filter.IsNullOrWhiteSpace(),
                    patient => patient.MasterPatientIndex.Contains(filter)
                 )
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }


        public async Task<List<HospitalPatient>> GetFilteredtListAsync(int skipCount, int maxResultCount, string sorting, DateTime? creationTime, Guid? cloudPatientId, string masterPatientIndex, string fhirPatientId)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                          .Where(data => (!string.IsNullOrEmpty(masterPatientIndex) ? data.MasterPatientIndex.StartsWith(masterPatientIndex) : true)
                                        && (cloudPatientId.HasValue ? data.CloudPatientId == cloudPatientId : true)
                                        && (!string.IsNullOrEmpty(fhirPatientId) ? data.FhirPatientId == fhirPatientId : true))
                          .OrderBy(sorting)
                          .Skip(skipCount)
                          .Take(maxResultCount)
                          .ToListAsync();
        }
    }
}