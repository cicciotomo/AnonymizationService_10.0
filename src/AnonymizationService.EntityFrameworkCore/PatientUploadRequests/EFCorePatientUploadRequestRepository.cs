using AnonymizationService.EntityFrameworkCore;
using AnonymizationService.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.PatientUploadRequests
{
    public class EFCorePatientUploadRequestRepository :
        EfCoreRepository<AnonymizationServiceDbContext, PatientUploadRequest, Guid>
        , IPatientUploadRequestRepository
    {
        public EFCorePatientUploadRequestRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<PatientUploadRequest>> GetListAsync(int skipCount, int maxResultCount, string sorting, string masterPatientIndex = null)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .WhereIf(
                    !masterPatientIndex.IsNullOrWhiteSpace(),
                    patient => patient.MasterPatientIndex == masterPatientIndex
                 )
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }

        public async Task<List<PatientUploadRequest>> GetListByBatchIdAsync(int skipCount, int maxResultCount, string sorting, Guid batchId)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(patient => patient.BatchRequestId == batchId)
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }


        public async Task<List<PatientUploadRequest>> GetListCreationTimeAsync(int skipCount, int maxResultCount, string sorting, DateTime? creationTime)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(patient => creationTime.HasValue ? patient.CreationTime.Date == creationTime.Value : true)
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }

        public async Task<List<PatientUploadRequest>> GetRunningListAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(r => r.Status == PatientUploadStatus.Running)
                .ToListAsync();
        }

        public async Task<List<PatientUploadRequest>> GetPendingListAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .Where(r => r.Status == PatientUploadStatus.Pending)
                .ToListAsync();
        }

        public async Task<List<PatientUploadRequest>> GetLastRequestForEachPatientListAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .GroupBy(r => r.MasterPatientIndex, (key, gr) => gr.OrderByDescending(r => r.CreationTime).First())
                .ToListAsync();
        }

        public async Task<List<PatientUploadRequest>> GetFilteredListAsync(int skipCount, int maxResultCount, string sorting, string? masterPatientIndex, DateTime? startCreationTimeFilter,
                                                                        DateTime? endCreationTimeFilter, bool? clinicalStateMachineShouldStart
                                                                        , bool? dicomStateMachineShouldStart, bool? laboratoryStateMachineShouldStart
                                                                        , bool? dbUriStateMachineShouldStart, bool? pathoxStateMachineShouldStart
                                                                        , bool? redcapStateMachineShouldStart, bool? intensiveCareStateMachineShouldStart)
        {


            var dbSet = await GetDbSetAsync();
            return await dbSet
                .AsNoTracking()
                .Where(patient =>
                    (!startCreationTimeFilter.HasValue || patient.CreationTime.Date >= startCreationTimeFilter.Value.Date) &&
                    (!endCreationTimeFilter.HasValue || patient.CreationTime.Date <= endCreationTimeFilter.Value.Date) &&
                    (string.IsNullOrEmpty(masterPatientIndex) || patient.MasterPatientIndex.StartsWith(masterPatientIndex)) &&
                    (clinicalStateMachineShouldStart == null || patient.ClinicalStateMachineShouldStart == clinicalStateMachineShouldStart) &&
                    (dbUriStateMachineShouldStart == null || patient.DbUriStateMachineShouldStart == dbUriStateMachineShouldStart) &&
                    (dicomStateMachineShouldStart == null || patient.DicomStateMachineShouldStart == dicomStateMachineShouldStart) &&
                    (laboratoryStateMachineShouldStart == null || patient.LaboratoryStateMachineShouldStart == laboratoryStateMachineShouldStart) &&
                    (pathoxStateMachineShouldStart == null || patient.PathoxStateMachineShouldStart == pathoxStateMachineShouldStart) &&
                    (redcapStateMachineShouldStart == null || patient.RedcapStateMachineShouldStart == redcapStateMachineShouldStart) &&
                    (intensiveCareStateMachineShouldStart == null || patient.IntensiveCareStateMachineShouldStart == intensiveCareStateMachineShouldStart) 
                )
                .OrderBy(sorting)
                .ToListAsync();


        }
    }
}
