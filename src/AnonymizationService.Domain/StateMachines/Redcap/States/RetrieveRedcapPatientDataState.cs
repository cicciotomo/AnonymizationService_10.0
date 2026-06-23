using AnonymizationService.Jobs.RetrieveRedcapData;
using DemographicWS;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Redcap.States
{
    public class RetrieveRedcapPatientDataState : State
    {
        public RetrieveRedcapPatientDataState()
        {

        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not RedcapStateMachine redcapStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for RetrieveRedcapPatientDataState", stateMachine.Id));
            }
            try
            {
                if (string.IsNullOrEmpty(redcapStateMachine.ContextData.RedcapStudyDefinition))
                {
                    throw new Exception(string.Format("[{0}] RedcapStudyConfigurationId not specified", stateMachine.Id));
                }
                //lancio job di gestione
                var jobScheduler = serviceProvider.GetService<IJobScheduler>();
                await jobScheduler.EnqueueJob<RetrieveRedcapPatientDataJob, RetrieveRedcapPatientDataJobArgs, JobResult>(new RetrieveRedcapPatientDataJobArgs()
                {
                    MasterPatientIndex = redcapStateMachine.ContextData.MasterPatientIndex,
                    StateMachineId = stateMachine.Id,
                    StartDate = redcapStateMachine.ContextData.StartDate,
                    CloudPatientId = redcapStateMachine.ContextData.CloudPatientId,
                    RedCapStudyId = Guid.Parse(redcapStateMachine.ContextData.RedcapStudyDefinition),
                    RedCapRecordId = redcapStateMachine.ContextData.RedCapID

                });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("[{0}] Error in RetrieveRedcapPatientDataState {1}", stateMachine.Id, ex.Message), ex);
            }

        }
    }
}


