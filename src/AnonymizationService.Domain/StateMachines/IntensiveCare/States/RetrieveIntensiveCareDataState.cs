using AnonymizationService.Jobs.RetrieveIntensiveCare;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.IntensiveCare.States
{
    public class RetrieveIntensiveCareDataState : State
    {
        public RetrieveIntensiveCareDataState()
        {

        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var jobScheduler = serviceProvider.GetService<IJobScheduler>();

            if (stateMachine is not IntensiveCareStateMachine intensiveCareStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            await jobScheduler.EnqueueJob<RetrieveIntensiveCareJob, RetrieveIntensiveCareJobArgs, JobResult>(new RetrieveIntensiveCareJobArgs()
            {
                MasterPatientIndex = intensiveCareStateMachine.ContextData.MasterPatientIndex,
                StateMachineId = stateMachine.Id,
                StartDate = intensiveCareStateMachine.ContextData.StartDate,
                NosologicalCodes = intensiveCareStateMachine.ContextData.NosologicalCodes,
                CloudPatientId = intensiveCareStateMachine.ContextData.CloudPatientId
            });
        }
    }
}
