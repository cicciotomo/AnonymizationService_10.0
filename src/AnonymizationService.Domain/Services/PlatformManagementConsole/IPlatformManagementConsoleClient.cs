using AnonymizationService.DicomData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AnonymizationService.Services.PlatformManagementConsole;

public interface IPlatformManagementConsoleClient
{
    Task RequestPatientDeletionAsync(Guid cloudPatientId);
    Task<List<StudyMetadata>> GetAvailableStudiesListAsync();
    Task<List<StudyMetadata>> GetAvailableStudiesWithCohortStausBozzaListAsync();
    Task<FullStudyMetadata> GetStudyAsync(Guid studyId);
    Task AddPatientToStudy(StudyPatient patient);
    Task RemovePatientFromStudy(StudyPatient patient);
}