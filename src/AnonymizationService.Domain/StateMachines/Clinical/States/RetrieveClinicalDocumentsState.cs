using AnonymizationService.Jobs.RetrieveClinicalDocuments;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Clinical.States
{
    public class RetrieveClinicalDocumentsState : State
    {
        public RetrieveClinicalDocumentsState()
        {

        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var jobScheduler = serviceProvider.GetService<IJobScheduler>();

            if (stateMachine is not ClinicalStateMachine clinicalStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            await jobScheduler.EnqueueJob<RetrieveClinicalDocumentsJob, RetrieveClinicalDocumentsJobArgs, JobResult>(new RetrieveClinicalDocumentsJobArgs()
            {
                MasterPatientIndex = clinicalStateMachine.ContextData.MasterPatientIndex,
                StateMachineId = stateMachine.Id,
                StartDate = clinicalStateMachine.ContextData.StartDate,
                DocumentType = clinicalStateMachine.ContextData.DocumentType,
                CloudPatientId = clinicalStateMachine.ContextData.CloudPatientId
            });
        }

    }
}
