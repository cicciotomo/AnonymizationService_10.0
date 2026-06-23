using AnonymizationService.StateMachines.IntensiveCare.States;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnonymizationService.StateMachines.IntensiveCare
{
    public class IntensiveCareStateMachine : ContextStateMachine<IntensiveCareStateMachineContext>
    {
        public override StateMachineEvent StateMachineCompletedEventType => new IntensiveCareDataStateMachineCompletedEvent();
        private IntensiveCareStateMachine() { }

        internal IntensiveCareStateMachine(Guid cloudPatientId, string masterPatientIndex, string fhirPatientId, Guid? parentStateMachineId, DateTime startDate, IEnumerable<string> nosologicalCodes)
        {
            ParentStateMachineId = parentStateMachineId;

            ContextData = new IntensiveCareStateMachineContext
            {
                FhirPatientId = fhirPatientId,
                MasterPatientIndex = masterPatientIndex,
                CloudPatientId = cloudPatientId,
                StartDate = startDate,
                NosologicalCodes = nosologicalCodes
            };
            Initialize();
        }

        public override State Initialize()
        {
            CurrentState = new RetrieveIntensiveCareDataState();

            SetCurrentStateValues();
            return CurrentState;
        }

        protected override State ReactToEvent(Event receivedEvent)
        {
            switch (CurrentState, receivedEvent)
            {
                case (RetrieveIntensiveCareDataState, IntensiveCareDocumentsRetrievedEvent intensiveCareDataCompletedEvent):
                    if (intensiveCareDataCompletedEvent.Documents.Any())
                    {
                        this.ContextData.RetrievedData = intensiveCareDataCompletedEvent.Documents;
                        CurrentState = new PersistIntensiveCareDataState();
                    }
                    else
                    {
                        SetCompleted();
                    }
                    break;
                case (PersistIntensiveCareDataState, IntensiveCareDocumentsPersistedEvent intensiveCareDocumentsPersistedEvent):
                    CurrentState = new UploadIntensiveCareDataToFhirState();
                    break;
                case (UploadIntensiveCareDataToFhirState, IntensiveCareDataSentToFHIREvent uploadCompletedEvent):
                    CurrentState = new UpdateIntensiveCareDataState(uploadCompletedEvent.IntensiveCarePatientDatas);
                    break;
                case (UpdateIntensiveCareDataState, IntensiveCareDataCheckUpdateCompletedEvent):
                    CurrentState = new CallSynapsePipelineState();
                    //SetCompleted();
                    break;
                case (CallSynapsePipelineState, SynapsePipelineCalledEvent):
                    SetCompleted();
                    break;
            }
            SetCurrentStateValues();
            return CurrentState;
        }
    }

    public class IntensiveCareStateMachineContext
    {
        public string FhirPatientId { get; set; }
        public Guid CloudPatientId { get; set; }
        public DateTime StartDate { get; set; }
        public string MasterPatientIndex { get; set; }
        public IEnumerable<string> NosologicalCodes { get; set; }
        public IEnumerable<IntensiveCareDocumentSummary> RetrievedData { get; set; }
    }
}
