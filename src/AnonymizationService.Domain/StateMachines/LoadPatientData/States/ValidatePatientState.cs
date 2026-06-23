using AnonymizationService.Jobs.ValidatePatient;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.LoadPatientData.States
{
    internal class ValidatePatientState : State
    {
        public ValidatePatientState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LoadPatientDataStateMachine patientDataStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            await JobScheduler.EnqueueJob<ValidatePatientJob, ValidatePatientJobArgs, JobResult>(new ValidatePatientJobArgs
            {
                MasterPatientIndex = patientDataStateMachine.ContextData.MasterPatientIndex,
                StateMachineId = patientDataStateMachine.Id
            });
        }
    }
}
