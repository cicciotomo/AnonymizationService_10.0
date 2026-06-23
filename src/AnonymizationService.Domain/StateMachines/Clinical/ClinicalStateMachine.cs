using AnonymizationService.ClinicalDocuments;
using AnonymizationService.StateMachines.Clinical.States;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.Clinical
{
    public class ClinicalStateMachine : ContextStateMachine<ClinicalStateMachineContext>
    {
        public override StateMachineEvent StateMachineCompletedEventType => new ClinicalDataStateMachineCompletedEvent();
        private ClinicalStateMachine() { }

        internal ClinicalStateMachine(Guid cloudPatientId, string masterPatientIndex, string fhirPatientId, Guid? parentStateMachineId, DateTime startDate, ClinicalDocumentTypeFilter[] documentType)
        {
            ParentStateMachineId = parentStateMachineId;

            ContextData = new ClinicalStateMachineContext
            {
                FhirPatientId = fhirPatientId,
                MasterPatientIndex = masterPatientIndex,
                CloudPatientId = cloudPatientId,
                StartDate = startDate,
                DocumentType = documentType
            };

            Initialize();
        }

        public override State Initialize()
        {
            CurrentState = new RetrieveClinicalDocumentsState();
            SetCurrentStateValues();
            return CurrentState;
        }

        protected override State ReactToEvent(Event receivedEvent)
        {
            switch (CurrentState, receivedEvent)
            {
                case (RetrieveClinicalDocumentsState, ClinicalDocumentsRetrievedEvent clinicalDataCompletedEvent):
                    if (clinicalDataCompletedEvent.Documents.Count > 0)
                    {
                        this.ContextData.ClinicalDocuments = clinicalDataCompletedEvent.Documents;
                        CurrentState = new PersistClinicalDocumentsState();
                    }
                    else
                    {
                        CurrentState = new UpdateClinicalUploadedState(new List<int>());
                    }
                    break;
                case (PersistClinicalDocumentsState, ClinicalDocumentsPersistedEvent):
                    CurrentState = new ExtractDataFromDocumentsState();
#if DEBUG
                    SetCompleted();
#endif
                    break;
                case (ExtractDataFromDocumentsState, DocumentsProcessedEvent documentsProcessedEvent):
                    CurrentState = new SendClinicalDataToAzureState(documentsProcessedEvent.ProcessResults);
                    break;
                case (SendClinicalDataToAzureState, ClinicalDataSentToAzureEvent clinicalDocumentsSentEvent):
                    CurrentState = new UpdateClinicalUploadedState(clinicalDocumentsSentEvent.UploadedDocumentsId);
                    break;
                case (UpdateClinicalUploadedState, ClinicalDataCheckUpdateCompletedEvent):
                    SetCompleted();
                    break;
            }
            SetCurrentStateValues();
            return CurrentState;
        }
    }

    public class ClinicalStateMachineContext
    {
        public string FhirPatientId { get; set; }
        public Guid CloudPatientId { get; set; }
        public DateTime StartDate { get; set; }
        public string MasterPatientIndex { get; set; }
        public ClinicalDocumentTypeFilter[] DocumentType { get; set; }
        public List<ClinicalDocumentSummary> ClinicalDocuments { get; set; }
    }
}
