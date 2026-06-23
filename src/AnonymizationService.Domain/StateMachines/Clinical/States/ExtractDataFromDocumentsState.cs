using AnonymizationService.ClinicalDocuments;
using AnonymizationService.DefaultImportParameters;
using AnonymizationService.Jobs.AnalyzeClinicalDocumentJob;
using AnonymizationService.PipelineExecutor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Clinical.States
{
    public class ExtractDataFromDocumentsState : State
    {


        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not ClinicalStateMachine clinicalStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var jobScheduler = serviceProvider.GetService<IJobScheduler>();
            var clinicalDocumentsRepository = serviceProvider.GetService<IRepository<ClinicalDocument>>();

            var alreadyProcessedDocumentsForPatient = await clinicalDocumentsRepository
                .GetListAsync(
                    e => e.CloudPatientId == clinicalStateMachine.ContextData.CloudPatientId
                    && e.CloudUploadDate != null);

            var documentsToProcess = clinicalStateMachine.ContextData.ClinicalDocuments
                .ExceptBy(alreadyProcessedDocumentsForPatient.Select(e => e.Id), e => e.Id)
                .ToList();
            
            await jobScheduler.EnqueueJob<AnalyzeClinicalDocumentJob, AnalyzeClinicalDocumentJobArgs, JobResult>(new AnalyzeClinicalDocumentJobArgs()
            {
                Documents = documentsToProcess,
                StateMachineId = stateMachine.Id
            });
        }
    }
}
