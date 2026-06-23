using AnonymizationService.EntityFrameworkCore;
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
    public class EfCorePatientBatchUploadRequestRepository : EfCoreRepository<AnonymizationServiceDbContext, PatientBatchUploadRequest, Guid>, IPatientBatchUploadRequestRepository
    {
        public EfCorePatientBatchUploadRequestRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<PatientBatchUploadRequest>> GetListAsync(int skipCount, int maxResultCount, string sorting)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }



        public async Task<List<PatientBatchUploadRequest>> GetFilteredListAsync(int skipCount, int maxResultCount, string sorting,DateTime? creationTime)
        {
            var dbSet = await GetDbSetAsync();
            if (creationTime.HasValue) { dbSet.Where(data => data.CreationTime.Date == creationTime.Value.Date); }
            return await dbSet
                .OrderBy(sorting)
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync();
        }

       
    }
}

