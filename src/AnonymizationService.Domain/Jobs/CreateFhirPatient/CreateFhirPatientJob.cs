using System;
using System.Linq;
using AnonymizationService.HospitalPatients;
using AnonymizationService.Services.FhirService;
using AnonymizationService.StateMachines.LoadPatientData;
using Porini.Abp.StateMachineEngine.Jobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AnonymizationService.Services.PlatformManagementConsole;
using Azure.Core;

namespace AnonymizationService.Jobs.CreateFhirPatient
{
    internal class CreateFhirPatientJob : Job<CreateFhirPatientArgs, JobResult>
    {
        private readonly IFhirServiceClient _fhirServiceClient;
        private readonly IHospitalPatientService _hospitalPatientService;
        private readonly IPlatformManagementConsoleClient _platformManagementConsoleClient;
        private readonly ILogger<CreateFhirPatientJob> _logger;

        public CreateFhirPatientJob(IFhirServiceClient fhirServiceClient
            , IHospitalPatientService hospitalPatientService
            , IPlatformManagementConsoleClient platformManagementConsoleClient
            , ILogger<CreateFhirPatientJob> logger)
        {
            _fhirServiceClient = fhirServiceClient;
            _hospitalPatientService = hospitalPatientService;
            _platformManagementConsoleClient = platformManagementConsoleClient;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(CreateFhirPatientArgs args)
        {
            _logger.LogInformation($"Upserting patient with cloud index {args.CloudPatientId}");

            var fhirPatient = await _fhirServiceClient.UpsertPatientAsync(args.CloudPatientId, args.Gender, args.BirthDate);
            await _hospitalPatientService.UpdateFhirIdForPatientAsync(args.CloudPatientId, fhirPatient.Id);

            if (args.FhirStudyIds?.Any() == true)
            {
                _logger.LogInformation($"Upserting patient-study relations for patient {args.CloudPatientId}");
                foreach (var fhirStudyId in args.FhirStudyIds)
                {
                    var studyId =  _platformManagementConsoleClient.GetStudyAsync(new Guid(fhirStudyId)).Result.Id.ToString();
                    var itemToAdd = new StudyPatient
                    {
                        CloudPatientIndex = args.CloudPatientId.ToString(),
                        IsDeleted = false,
                        IsFromCohortBuilder = false,
                        StudyId = studyId
                    };
                    await _platformManagementConsoleClient.AddPatientToStudy(itemToAdd);
                }
            }

            return new JobResult
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new FhirPatientCreatedEvent(fhirPatient.Id)
            };
        }
    }
}
