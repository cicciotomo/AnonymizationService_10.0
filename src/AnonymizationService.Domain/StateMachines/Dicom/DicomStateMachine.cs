using AnonymizationService.DicomData;
using AnonymizationService.Services.Dicom;
using AnonymizationService.StateMachines.Dicom.States;
using AnonymizationService.StateMachines.LoadPatientData;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AnonymizationService.StateMachines.Dicom
{
    public class DicomStateMachine : ContextStateMachine<DicomStateMachineContext>
    {
        public override StateMachineEvent StateMachineCompletedEventType => new DicomStateMachineCompletedEvent();

        private DicomStateMachine() { }
        public DicomStateMachine(Guid cloudPatientId, string masterPatientIndex, Guid? parentStateMachineId, DateTime startDate, DateTime endDate, string[] modalities, string pacsSource, DicomParametersJson parametersJson)
        {
            ParentStateMachineId = parentStateMachineId;
            ContextData = new DicomStateMachineContext
            {
                StartDate = startDate,
                EndDate = endDate,
                Modalities = modalities,
                CloudPatientId = cloudPatientId,
                MasterPatientIndex = masterPatientIndex,
                PacsSource = pacsSource,
                DicomParametersJson = parametersJson
            };
            Initialize();
        }

        public override State Initialize()
        {
            CurrentState = new RetrieveDicomDataState();
            SetCurrentStateValues();
            return CurrentState;
        }

        protected override State ReactToEvent(Event @event)
        {
            switch (CurrentState, @event)
            {
                case (RetrieveDicomDataState, DicomDataRetrievedEvent dicomStudiesRetrievedEvent):

                    if (dicomStudiesRetrievedEvent.DicomStudies.Count > 0)
                    {
                        this.ContextData.DicomStudies = dicomStudiesRetrievedEvent.DicomStudies;
                        CurrentState = new PersistDicomDataState();
                    }
                    else
                    {
                        CurrentState = new UpdateDicomUploadedState();
                    }
                    break;
                case (RetrieveDicomDataState, DicomDataMovedToPacsRicercaEvent dicomDataMovedToPacsRicercaEvent):
#if DEBUG
                    SetCompleted();
                    break;
#else
                    CurrentState = new AnonymizeImagesState();
                    break;   
#endif
                case (PersistDicomDataState, PersistDicomDataStateEvent):
                    CurrentState = new AnonymizeImagesState();
                    break;

                case (AnonymizeImagesState, DicomDataAnonymizedEvent dicomDataAnonymizedEvent):
                    CurrentState = new SendDicomToAzureState(dicomDataAnonymizedEvent.DicomStudies);
                    break;
                case (SendDicomToAzureState, DicomDataSentToAzureEvent dataSentToAzure):
                    CurrentState = new UpdateDicomUploadedState();
                    break;
                case (UpdateDicomUploadedState, DicomDataCheckUpdateCompletedEvent):
                    SetCompleted();
                    break;
            }
            SetCurrentStateValues();
            return CurrentState;
        }
    }

    public class DicomStateMachineContext
    {
        public Guid CloudPatientId { get; set; }
        public string MasterPatientIndex { get; set; }
        public string[] Modalities { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DicomData.DicomStudy> DicomStudies { get; set; }
        public string PacsSource { get; set; }
        public DicomParametersJson DicomParametersJson { get; set; }

    }
}
