using AnonymizationService.Jobs.SendClinicalDataToAzure;
using AnonymizationService.Jobs.UpsertFhirRedcapPatientData;
using AnonymizationService.StateMachines.Clinical;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Redcap.States
{
    internal class SendRedcapPatientDataToFHIRState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not RedcapStateMachine redcapStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for SendRedcapPatientDataToFHIRState", stateMachine.Id));
            }
            try
            {
                // lancio job di gestione
                var jobScheduler = serviceProvider.GetService<IJobScheduler>();
                await jobScheduler.EnqueueJob<UpsertFhirRedcapPatientDataJob, UpsertFhirRedcapPatientDataArgs, JobResult>
                    (new UpsertFhirRedcapPatientDataArgs()
                {
                    CloudPatientId = redcapStateMachine.ContextData.CloudPatientId,
                    FhirPatientId = redcapStateMachine.ContextData.FhirPatientId,
                    Data = redcapStateMachine.ContextData.Summary.JsonResponseString,
                    StateMachineId = stateMachine.Id,
                    Metadata = redcapStateMachine.ContextData.Summary.Metadata,
                    MetadataFhirId = redcapStateMachine.ContextData.Summary.MetadataFhirId,
                    RedcapStudyConfigurationID = Guid.Parse(redcapStateMachine.ContextData.RedcapStudyDefinition),
                    StudyName = redcapStateMachine.ContextData.Summary.Study_name,
                    Token = redcapStateMachine.ContextData.Summary.Token
                    });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("[{0}] Error in SendRedcapPatientDataToFHIRState {1}", stateMachine.Id, ex.Message), ex);
            }


        }
    }
}
