using AnonymizationService.Jobs.SendClinicalDataToAzure;
using AnonymizationService.Jobs.UpsertFhirRedcapPatientData;
using AnonymizationService.Jobs.UpsertFihrRedcapStudyMetadata;
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
    internal class SendRedcapMetaDataToFHIRState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not RedcapStateMachine redcapStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for SendRedcapMetaDataToFHIRState", stateMachine.Id));
            }
            try
            {
                // lancio job di gestione
                var jobScheduler = serviceProvider.GetService<IJobScheduler>();
                await jobScheduler.EnqueueJob<UpsertFihrRedcapStudyMetadataJob, UpsertFihrRedcapStudyMetadataArgs, JobResult>
                    (new UpsertFihrRedcapStudyMetadataArgs()
                    {
                        StateMachineId = stateMachine.Id,
                        Metadata = redcapStateMachine.ContextData.Summary.Metadata,
                        RedcapStudyConfigurationId = Guid.Parse(redcapStateMachine.ContextData.RedcapStudyDefinition)
                    });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("[{0}] Error in SendRedcapMetaDataToFHIRState {1}", stateMachine.Id, ex.Message), ex);
            }


        }
    }
}