using AnonymizationService.Jobs.RetrieveLaboratoryExams;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Laboratory.States
{
    internal class RetrieveLaboratoryExamsState : State
    {
        public RetrieveLaboratoryExamsState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LaboratoryStateMachine laboratoryStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            await JobScheduler.EnqueueJob<RetrieveLaboratoryExamsJob, RetrieveLaboratoryExamsArgs, JobResult>(new RetrieveLaboratoryExamsArgs()
            {
                MasterPatientIndex = laboratoryStateMachine.ContextData.MasterPatientIndex,
                StateMachineId = stateMachine.Id,
                StartDate = laboratoryStateMachine.ContextData.StartDate,
                CloudPatientId = laboratoryStateMachine.ContextData.CloudPatientId
            });
        }
    }
}
