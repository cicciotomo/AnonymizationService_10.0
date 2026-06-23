using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using System.Runtime.CompilerServices;
using AnonymizationService.Services.IntensiveCare;
using AnonymizationService.StateMachines.IntensiveCare;
using System.Diagnostics.Metrics;

namespace AnonymizationService.Jobs.RetrieveIntensiveCare
{
    internal class RetrieveIntensiveCareJob : Job<RetrieveIntensiveCareJobArgs, JobResult>
    {
        private readonly IIntensiveCareService _intensiveCareService;
        private readonly ILogger<RetrieveIntensiveCareJob> _logger;

        public RetrieveIntensiveCareJob(
                IIntensiveCareService intensiveCareService,
                ILogger<RetrieveIntensiveCareJob> logger
            )
        {
            _intensiveCareService = intensiveCareService;
            _logger = logger;
        }

        /* pipeline(PatientId, CPI, StartDate, EndDate)
         * 
         * 1) (RETRIEVEPATIENTDATA) recuperare PatientData e ENCOUNTERT da NOSOLOGICO+STARTDATE
         * 2) (PERSISTDATAONDB) salvare REQUEST precedenti relative ad ENCOUNTER (solo encounter nuovi)
         * 3) (SENDDATATOFHIR ) send ENCOUNTERS to FHIR + update CloudUploadDate
         * 4) (SENDTOSYNAPSE) invocare PIPELINE solo encounter nuovi + update PipelineID, PipelineCompletedDate, PipelineReturnMessage
         * 5) COMPLETE
        */
        public override async Task<JobResult> ExecuteJobAsync(RetrieveIntensiveCareJobArgs args)
        {
            _logger.LogInformation("Retrieving intensive care data for nosological codes ");


            var retrievedPatientData = await _intensiveCareService.GetPatientsFromNosologici(args.NosologicalCodes);

            var retrievedDocuments = new List<IntensiveCareDocumentSummary>();


            foreach (var patient in retrievedPatientData)
            {
                var encouters = await _intensiveCareService.GetEncountersAsync(patient.PatientId);
                encouters = encouters.Where(e => e.ExamStartDate > args.StartDate).ToList();

                IntensiveCareDocumentSummary item = new IntensiveCareDocumentSummary();

                item.PatientId = patient.PatientId;
                item.Timestamp = patient.Timestamp;
                item.Value = patient.Value;
                item.Name = patient.Name;
                item.Encounters = encouters;
                retrievedDocuments.Add(item);
            }
            
            _logger.LogInformation($"Aggiunti {retrievedDocuments.Count()} PatientId da elaborare");
            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new IntensiveCareDocumentsRetrievedEvent(retrievedDocuments)
            };
        }
    }
}
