using AnonymizationService.Jobs.SendClinicalDataToAzure;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Clinical.States
{
    public class SendClinicalDataToAzureState : StatefulState<List<ParsedClinicalDocument>>
    {
        public SendClinicalDataToAzureState() { }

        public SendClinicalDataToAzureState(List<ParsedClinicalDocument> parsedClinicalDocuments)
        {
            this.StateData = parsedClinicalDocuments;
        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var jobScheduler = serviceProvider.GetService<IJobScheduler>();

            if (stateMachine is not ClinicalStateMachine clinicalStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            await jobScheduler.EnqueueJob<SendClinicalDataToAzureJob, SendClinicalDataToAzureArgs, JobResult>(new SendClinicalDataToAzureArgs()
            {
                CloudPatientId = clinicalStateMachine.ContextData.CloudPatientId,
                FhirPatientId = clinicalStateMachine.ContextData.FhirPatientId,
                SerializedAnalyticsResults = this.StateData,
                StateMachineId = stateMachine.Id
            });
        }
    }
}
