using AnonymizationService.EventTracker;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.LoadPatientData.States
{
    public class PatientValidationFailedState : State
    {
        public PatientValidationFailedState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LoadPatientDataStateMachine patientDataStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var eventTracker = serviceProvider.GetService<IEventTracker>();
            eventTracker.TrackEvent<string>("PatientValidationFailed", patientDataStateMachine.ContextData.MasterPatientIndex);

            await LocalEventBus.PublishAsync(new PatientValidationFailedHandledEvent(stateMachine.Id));
            await PublishEventOnBusAsync(new ExecutedTaskEvent(patientDataStateMachine.Id, new PatientValidationFailedHandledEvent(stateMachine.Id)));
        }
    }
}
