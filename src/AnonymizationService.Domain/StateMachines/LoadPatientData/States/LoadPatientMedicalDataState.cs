using AnonymizationService.StateMachines.Clinical;
using AnonymizationService.StateMachines.DbUri;
using AnonymizationService.StateMachines.Dicom;
using AnonymizationService.StateMachines.Laboratory;
using AnonymizationService.StateMachines.Pathox;
using AnonymizationService.StateMachines.IntensiveCare;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;
using AnonymizationService.StateMachines.Redcap;

namespace AnonymizationService.StateMachines.LoadPatientData.States
{
    internal class LoadPatientMedicalDataState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var stateMachineRepository = serviceProvider.GetService<IStateMachineRepository>();

            if (stateMachine is not LoadPatientDataStateMachine patientStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var stateMachineManager = serviceProvider.GetRequiredService<StateMachineManager>();

            var loadPatientMedicalDataStateContent = new LoadPatientMedicalDataStateContent();

            if (patientStateMachine.ContextData.DicomParameters?.StateMachineShouldStart == true)
            {
                var dicomStateMachine = new DicomStateMachine(
                    patientStateMachine.ContextData.CloudPatientId.Value
                    , patientStateMachine.ContextData.MasterPatientIndex
                    , stateMachine.Id
                    , (DateTime)patientStateMachine.ContextData.DicomParameters.StartDate

                    , (DateTime)patientStateMachine.ContextData.DicomParameters.EndDate
                    , patientStateMachine.ContextData.DicomParameters.DicomModalities
                    , patientStateMachine.ContextData.DicomParameters.PacsSource
                    , patientStateMachine.ContextData.DicomParameters.DicomParametersJson);

                await stateMachineRepository.InsertAsync(dicomStateMachine);
                await stateMachineManager.TryRunNewStateMachineAsync(dicomStateMachine);
                loadPatientMedicalDataStateContent.OnWaitingDicomDataDownloadCompleted = true;
            }

            if (patientStateMachine.ContextData.ClinicalParameters?.StateMachineShouldStart == true)
            {
                var clinicalStateMachine = new ClinicalStateMachine(patientStateMachine.ContextData.CloudPatientId.Value
                    , patientStateMachine.ContextData.MasterPatientIndex
                    , patientStateMachine.ContextData.FhirPatientId
                    , stateMachine.Id
                    , (DateTime)patientStateMachine.ContextData.ClinicalParameters.StartDate
                    , patientStateMachine.ContextData.ClinicalParameters.DocumentType);

                await stateMachineRepository.InsertAsync(clinicalStateMachine);
                await stateMachineManager.TryRunNewStateMachineAsync(clinicalStateMachine);
                loadPatientMedicalDataStateContent.OnWaitingClinicalDataDownloadCompleted = true;
            }

            if (patientStateMachine.ContextData.LaboratoryParameters?.StateMachineShouldStart == true)
            {
                var laboratoryStateMachine = new LaboratoryStateMachine(patientStateMachine.ContextData.CloudPatientId.Value
                    , patientStateMachine.ContextData.MasterPatientIndex
                    , patientStateMachine.ContextData.FhirPatientId
                    , stateMachine.Id
                    , (DateTime)patientStateMachine.ContextData.LaboratoryParameters.StartDate);

                await stateMachineRepository.InsertAsync(laboratoryStateMachine);
                await stateMachineManager.TryRunNewStateMachineAsync(laboratoryStateMachine);
                loadPatientMedicalDataStateContent.OnWaitingLaboratoryDataDownloadCompleted = true;
            }

			if (patientStateMachine.ContextData.DbUriParameters?.StateMachineShouldStart == true)
			{
				var dbUriStateMachine = new DbUriStateMachine(patientStateMachine.ContextData.CloudPatientId.Value
					, patientStateMachine.ContextData.TaxCode
					, stateMachine.Id
                    , patientStateMachine.ContextData.FhirPatientId
					, patientStateMachine.ContextData.MasterPatientIndex
                    , (DateTime)patientStateMachine.ContextData.DbUriParameters.StartDate);
				
				await stateMachineRepository.InsertAsync(dbUriStateMachine);
				await stateMachineManager.TryRunNewStateMachineAsync(dbUriStateMachine);
				loadPatientMedicalDataStateContent.OnWaitingDbUriDataDownloadCompleted = true;
			}

			if (patientStateMachine.ContextData.PathoxParameters?.StateMachineShouldStart == true)
			{
				var pathoxStateMachine = new PathoxStateMachine(patientStateMachine.ContextData.CloudPatientId.Value
					, stateMachine.Id
					, patientStateMachine.ContextData.FhirPatientId
					, patientStateMachine.ContextData.MasterPatientIndex
					, (DateTime)patientStateMachine.ContextData.PathoxParameters.StartDate);

				await stateMachineRepository.InsertAsync(pathoxStateMachine);
				await stateMachineManager.TryRunNewStateMachineAsync(pathoxStateMachine);
				loadPatientMedicalDataStateContent.OnWaitingPathoxDataDownloadCompleted = true;
			}

            if (patientStateMachine.ContextData.IntensiveCareParameters?.StateMachineShouldStart == true)
            {
                var intensiveCareStateMachine = new IntensiveCareStateMachine(patientStateMachine.ContextData.CloudPatientId.Value
                    , patientStateMachine.ContextData.MasterPatientIndex
                    , patientStateMachine.ContextData.FhirPatientId
                    , stateMachine.Id
                    , (DateTime)patientStateMachine.ContextData.IntensiveCareParameters.StartDate
                    , patientStateMachine.ContextData.IntensiveCareParameters.NosologicalCodes);

                await stateMachineRepository.InsertAsync(intensiveCareStateMachine);
                await stateMachineManager.TryRunNewStateMachineAsync(intensiveCareStateMachine);
                loadPatientMedicalDataStateContent.OnWaitingIntensiveCareDataDownloadCompleted = true;
            }

            if (patientStateMachine.ContextData.RedcapParameters?.StateMachineShouldStart == true)
            {
                var redcapStateMachine = new RedcapStateMachine(
                      patientStateMachine.ContextData.CloudPatientId.Value
                    , patientStateMachine.ContextData.MasterPatientIndex
                    , patientStateMachine.ContextData.FhirPatientId
                    , stateMachine.Id
                    , (DateTime)patientStateMachine.ContextData.RedcapParameters.StartDate
                    , patientStateMachine.ContextData.RedcapParameters.RedcapStudyConfigurationId
                    , patientStateMachine.ContextData.RedcapParameters.RecordId
                    );

                await stateMachineRepository.InsertAsync(redcapStateMachine);
                await stateMachineManager.TryRunNewStateMachineAsync(redcapStateMachine);
                loadPatientMedicalDataStateContent.OnWaitingRedcapDataDownloadCompleted = true;
            }

            await PublishEventOnBusAsync(new ExecutedTaskEvent(patientStateMachine.Id, new LoadAllPatientDataEvent(loadPatientMedicalDataStateContent)));
        }
    }
}
