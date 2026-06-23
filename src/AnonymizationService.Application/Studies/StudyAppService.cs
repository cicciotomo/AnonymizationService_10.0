using System;
using AnonymizationService.Permissions;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnonymizationService.Services.PlatformManagementConsole;
using Microsoft.Extensions.Logging;
using Volo.Abp.ObjectMapping;
using AnonymizationService.HospitalPatients;
using System.Linq;

namespace AnonymizationService.Studies
{
    [Authorize(AnonymizationServicePermissions.PatientManagementPermission)]
    public class StudyAppService : AnonymizationServiceAppService
    {
        private readonly IPlatformManagementConsoleClient _platformManagementConsoleClient;
        private readonly IPatientAppService _patientAppService;

        public StudyAppService(IPlatformManagementConsoleClient platformManagementConsoleClient
            , IPatientAppService patientAppService)
        {
            _platformManagementConsoleClient = platformManagementConsoleClient;
            _patientAppService = patientAppService;
        }

        [AllowAnonymous]
        public async Task<List<StudyMetadataDto>> GetAvailableStudies()
        {
            try
            {
                var availableStudies = await _platformManagementConsoleClient.GetAvailableStudiesListAsync();
                var studyDtos = ObjectMapper.Map<List<StudyMetadata>, List<StudyMetadataDto>>(availableStudies);
                return studyDtos;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
                return new List<StudyMetadataDto>();
            }
        }

        [AllowAnonymous]
        public async Task<List<StudyMetadataDto>> GetAvailableStudiesWithCohortStausBozza()
        {
            try
            {
                var availableStudies = await _platformManagementConsoleClient.GetAvailableStudiesWithCohortStausBozzaListAsync();
                var studyDtos = ObjectMapper.Map<List<StudyMetadata>, List<StudyMetadataDto>>(availableStudies);
                return studyDtos;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
                return new List<StudyMetadataDto>();
            }
        }

        [AllowAnonymous]
        public async Task<FullStudyMetadataDto> GetStudyById(Guid studyId)
        {
            try
            {
                var study = await _platformManagementConsoleClient.GetStudyAsync(studyId);

                if (study.Patients != null) {
                    foreach (var p in study.Patients)
                    {
                        p.MPI = await _patientAppService.GetMPI(p.CPI);
                    }
                    //study.Patients = study.Patients.OrderBy(p => p.MPI).ToList();
                }
               
                var studyDto = ObjectMapper.Map<FullStudyMetadata, FullStudyMetadataDto>(study);
                return studyDto;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
                return new FullStudyMetadataDto();
            }
        }

        [AllowAnonymous]
        public async Task<List<StudyMetadataDto>> GetPatientListByCohorteId(Guid cohortId)
        {
            try
            {
                var availableStudies = await _platformManagementConsoleClient.GetAvailableStudiesListAsync();
                var studyDtos = ObjectMapper.Map<List<StudyMetadata>, List<StudyMetadataDto>>(availableStudies);
                return studyDtos;
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
                return new List<StudyMetadataDto>();
            }
        }

        [AllowAnonymous]
        public async Task<List<string>> AddPatientToStudyAsync(PatientsListToAddToStudyRequestDto request)
        {
            List<string> result = new List<string>();
            try
            {            
                foreach (var mpi in request.MasterPatientIds)
                {
                    var cpi = await _patientAppService.GetCPI(mpi);
                    if (cpi != null)
                    {
                        var itemToAdd = new StudyPatient
                        {
                            CloudPatientIndex = cpi.ToString(),
                            IsDeleted = false,
                            IsFromCohortBuilder = false,
                            StudyId = request.StudyFhirId.ToString()
                        };
                        await _platformManagementConsoleClient.AddPatientToStudy(itemToAdd);
                    }
                    else {
                        result.Add(mpi);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
            }
            return result;
        }

        [AllowAnonymous]
        public async Task RemovePatientFromStudyAsync(RemovePatientFromStudyRequestDto request)
        {
            try
            {
                var cpi = await _patientAppService.GetCPI(request.MasterPatientId);
                var itemToRemove = new StudyPatient
                {
                    CloudPatientIndex = cpi.ToString(),
                    StudyId = request.StudyId.ToString()
                };
                await _platformManagementConsoleClient.RemovePatientFromStudy(itemToRemove);

            }
            catch (Exception e)
            {
                Logger.LogWarning(e, e.Message);
            }

        }
    }
}
