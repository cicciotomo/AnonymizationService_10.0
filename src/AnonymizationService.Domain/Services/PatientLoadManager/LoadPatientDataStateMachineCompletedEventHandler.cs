using AnonymizationService.Localization;
using AnonymizationService.StateMachines.LoadPatientData;
using Microsoft.Extensions.Localization;
using Porini.Abp.StateMachineEngine.Events;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace AnonymizationService.Services.PatientLoadManager
{
    public class LoadPatientDataStateMachineCompletedEventHandler
        : ILocalEventHandler<CompletedStateMachineEvent>,
          ILocalEventHandler<FailedStateMachineEvent>,
          ILocalEventHandler<PatientValidationFailedHandledEvent>,
          ITransientDependency
    {
        private readonly IPatientLoadManager _patientLoadManager;
        private readonly IStringLocalizer<AnonymizationServiceResource> _stringLocalizer;

        public LoadPatientDataStateMachineCompletedEventHandler(IPatientLoadManager patientLoadManager, IStringLocalizer<AnonymizationServiceResource> stringLocalizer)
        {
            _patientLoadManager = patientLoadManager;
            _stringLocalizer = stringLocalizer;
        }

        public Task HandleEventAsync(CompletedStateMachineEvent eventData)
            => _patientLoadManager.HandleStateMachineCompletionAsync(eventData.StateMachineId);

        public Task HandleEventAsync(FailedStateMachineEvent eventData)
            => _patientLoadManager.HandleStateMachineCompletionAsync(eventData.StateMachineId, eventData.ErrorMessage);

        public Task HandleEventAsync(PatientValidationFailedHandledEvent eventData)
            => _patientLoadManager.HandleStateMachineCompletionAsync(eventData.StateMachineId, _stringLocalizer[AnonymizationServiceResource.MpiValidationFailedMessage]);

    }
}
