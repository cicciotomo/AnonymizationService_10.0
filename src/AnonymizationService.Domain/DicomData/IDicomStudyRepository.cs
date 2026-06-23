using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.DicomData
{
    public interface IDicomStudyRepository : IRepository<DicomStudy>
    {
        Task UpsertDiffAsync(List<DicomStudy> dicomStudies);
        Task<DicomStudy> GetStudyWithSeriesAsync(string studyId);
    }
}
