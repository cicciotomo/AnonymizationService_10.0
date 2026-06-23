using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using AnonymizationService.Services.FhirService.Adapters.DbUri;
using AnonymizationService.Services.FhirService;
using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnonymizationService.StateMachines.DbUri;
using EnsureThat;
using AnonymizationService.StateMachines.IntensiveCare;
using AnonymizationService.IntensiveCareData;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Hl7.Fhir.Model;

namespace AnonymizationService.Jobs.UpsertFhirIntensiveCarePatientData
{
    internal class UpsertFhirIntensiveCareDataJob : Job<UpsertFhirIntensiveCareDataJobArgs, JobResult>
    {
        private readonly IFhirServiceClient _fhirServiceClient;
        private readonly ILogger<UpsertFhirIntensiveCareDataJob> _logger;

        public UpsertFhirIntensiveCareDataJob(IFhirServiceClient fhirServiceClient, ILogger<UpsertFhirIntensiveCareDataJob> logger, IServiceProvider serviceProvider)
        {
            _fhirServiceClient = fhirServiceClient;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(UpsertFhirIntensiveCareDataJobArgs args)
        {
            _logger.LogInformation($"Uploading FHIR Resources for intensive care data of patients with FHIR {args.FhirPatientId}");

            var patientToSendEncounterOnFhir = args.PatientData.Where(p => !p.CloudUploadDate.HasValue).ToList();
            var patientAlreadySentEncounterOnFhir = args.PatientData.Where(p => p.CloudUploadDate.HasValue).ToList();

            foreach (var p in patientToSendEncounterOnFhir)
            {
                //gestire il caso in cui l'encounter sia stato trasmesso precedentemente e la relativa pipelineRun sia fallita
                try
                {
                    var e = await _fhirServiceClient.UpsertEncounter(p.ExamStartDate.UtcDateTime, p.ExamEndDate.HasValue ? p.ExamEndDate.Value.UtcDateTime : p.ExamStartDate.UtcDateTime, p.CloudPatientId.ToString(), args.FhirPatientId, Guid.NewGuid().ToString());
                    if (e is not null && e.Id is not null)
                    {
                        p.CloudUploadDate = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex ,LogLevel.Error);
                    _logger.LogError($"Errore durante il trasferimento per l'encounter con id={p.Id}");                 
                }
            }
            args.PatientData = patientAlreadySentEncounterOnFhir.Union(patientToSendEncounterOnFhir).ToList();
            
            return new JobResult()
            {
                JobResultEvent = new IntensiveCareDataSentToFHIREvent(args.PatientData),
                StateMachineId = args.StateMachineId
            };
        }
    }
}
