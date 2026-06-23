using AnonymizationService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.DicomData
{
    public class EfCoreDicomStudyRepository : EfCoreRepository<AnonymizationServiceDbContext, DicomStudy, string>, IDicomStudyRepository
    {
        public EfCoreDicomStudyRepository(IDbContextProvider<AnonymizationServiceDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task UpsertDiffAsync(List<DicomStudy> dicomStudies)
        {
            var dicomStudiesIds = dicomStudies.Select(st => st.Id).ToList();

            var dbSet = await GetDbSetAsync();

            var existingStudies = await dbSet
                  .Where(st => dicomStudiesIds.Contains(st.Id))
                  .Include(st => st.DicomSeries)
                  .ToListAsync();

            var existingStudiesIds = existingStudies.Select(se => se.Id).ToList();

            var existingSeriesIds = existingStudies
                .SelectMany(st => st.DicomSeries)
                .Select(se => se.Id)
                .ToList();


            foreach (var study in existingStudies)
            {
                var relatedSeriesToAdd = dicomStudies.SelectMany(st => st.DicomSeries)
                     .Where(se => se.DicomStudyId == study.Id && !existingSeriesIds.Contains(se.Id))
                     .ToList();
                study.AddDicomSeries(relatedSeriesToAdd);
                study.UpdateStudyFilePath(dicomStudies.Where(st => st.Id == study.Id).FirstOrDefault().StudyFilePath);
            }

            dbSet.UpdateRange(existingStudies);
            await dbSet.AddRangeAsync(dicomStudies.Where(st => !existingStudiesIds.Contains(st.Id)));

        }

        public async Task<DicomStudy> GetStudyWithSeriesAsync(string studyId)
        {
            var dbSet = await GetDbSetAsync();

            return await dbSet
                  .Where(st => st.Id == studyId)
                  .Include(st => st.DicomSeries)
                  .FirstOrDefaultAsync();
        }
    }

}
