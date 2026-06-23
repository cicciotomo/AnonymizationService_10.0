using AnonymizationService.StateMachines.Clinical;
using AnonymizationService.StateMachines.DbUri;
using AnonymizationService.StateMachines.Dicom;
using AnonymizationService.StateMachines.IntensiveCare;
using AnonymizationService.StateMachines.Laboratory;
using AnonymizationService.StateMachines.Pathox;
using AnonymizationService.StateMachines.Redcap;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.LoadPatientData.States
{
    public class WaitChildrenStateMachinesCompletionState : StatefulState<LoadPatientMedicalDataStateContent>
    {
        public WaitChildrenStateMachinesCompletionState() { }
        public WaitChildrenStateMachinesCompletionState(LoadPatientMedicalDataStateContent loadPatientMedicalDataStateContent)
        {
            this.StateData = loadPatientMedicalDataStateContent;
        }
        public override void HandleEvent(Event receivedEvent)
        {
            switch (receivedEvent)
            {
                case DicomStateMachineCompletedEvent:
                    this.StateData.OnWaitingDicomDataDownloadCompleted = false;
                    break;
                case LaboratoryExamsStateMachineCompletedEvent:
                    this.StateData.OnWaitingLaboratoryDataDownloadCompleted = false;
                    break;
                case ClinicalDataStateMachineCompletedEvent:
                    this.StateData.OnWaitingClinicalDataDownloadCompleted = false;
                    break;
				case DbUriDataStateMachineCompletedEvent:
					this.StateData.OnWaitingDbUriDataDownloadCompleted = false;
					break;
				case PathoxDataStateMachineCompletedEvent:
					this.StateData.OnWaitingPathoxDataDownloadCompleted = false;
					break;
                case IntensiveCareDataStateMachineCompletedEvent:
                    this.StateData.OnWaitingIntensiveCareDataDownloadCompleted = false;
                    break;
                case RedcapDataStateMachineCompletedEvent:
                    this.StateData.OnWaitingRedcapDataDownloadCompleted = false;
                    break;
            }
        }

        public override bool CanGoNext() =>
            !this.StateData.OnWaitingDicomDataDownloadCompleted
            && !this.StateData.OnWaitingLaboratoryDataDownloadCompleted
            && !this.StateData.OnWaitingClinicalDataDownloadCompleted
			&& !this.StateData.OnWaitingDbUriDataDownloadCompleted
			&& !this.StateData.OnWaitingPathoxDataDownloadCompleted
            && !this.StateData.OnWaitingIntensiveCareDataDownloadCompleted
            && !this.StateData.OnWaitingRedcapDataDownloadCompleted;

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
        }
    }
    public class LoadPatientMedicalDataStateContent
    {
        public bool OnWaitingDicomDataDownloadCompleted { get; set; }
        public bool OnWaitingLaboratoryDataDownloadCompleted { get; set; }
        public bool OnWaitingClinicalDataDownloadCompleted { get; set; }
		public bool OnWaitingDbUriDataDownloadCompleted { get; set; }
		public bool OnWaitingPathoxDataDownloadCompleted { get; set; }
        public bool OnWaitingIntensiveCareDataDownloadCompleted { get; set; }
        public bool OnWaitingRedcapDataDownloadCompleted { get; set; }
    }
}
