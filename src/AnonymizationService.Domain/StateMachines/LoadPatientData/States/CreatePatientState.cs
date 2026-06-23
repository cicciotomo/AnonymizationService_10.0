using AnonymizationService.HospitalPatients;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.LoadPatientData.States
{
    internal class CreatePatientState : State
    {
        public CreatePatientState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LoadPatientDataStateMachine loadPatientDataStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var patientService = serviceProvider.GetRequiredService<HospitalPatientService>();
            var patient = await patientService.CreatePatientAsync(loadPatientDataStateMachine.ContextData.MasterPatientIndex);
            await PublishEventOnBusAsync(new ExecutedTaskEvent(loadPatientDataStateMachine.Id, new PatientCreatedEvent(patient.CloudPatientId)));
        }
    }
}
