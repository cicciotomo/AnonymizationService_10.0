using AnonymizationService.StateMachines.Redcap.States;
using Microsoft.Identity.Client;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;

namespace AnonymizationService.StateMachines.Redcap
{
    public class RedcapStateMachine : ContextStateMachine<RedcapStateMachineContext>
    {
        public override StateMachineEvent StateMachineCompletedEventType => new RedcapDataStateMachineCompletedEvent();
        private RedcapStateMachine() { }

        internal RedcapStateMachine(Guid cloudPatientId, string masterPatientIndex, string fhirPatientId, Guid? parentStateMachineId
            , DateTime startDate, string redcapStudyId, string recordId)
        {
            ParentStateMachineId = parentStateMachineId;

            ContextData = new RedcapStateMachineContext
            {
                StateMachineID = this.Id,
                FhirPatientId = fhirPatientId,
                MasterPatientIndex = masterPatientIndex,
                CloudPatientId = cloudPatientId,
                StartDate = startDate,
                RedcapStudyDefinition = redcapStudyId,
                RedCapID = recordId
            };
            Initialize();
        }

        public override State Initialize()
        {
            CurrentState = new RetrieveRedcapPatientDataState();

            SetCurrentStateValues();
            return CurrentState;
        }

        protected override State ReactToEvent(Event receivedEvent)
        {
            switch (CurrentState, receivedEvent)
            {
                case (RetrieveRedcapPatientDataState, RedcapPatientDataRetrievedEvent evento):
                    if(evento.Data is not null) {
                        ContextData.Summary = evento.Data;
                        CurrentState = new PersistRedcapPatientDataState();
                    }
                    else
                    {
                        SetCompleted();
                    }
                    break;
                case (PersistRedcapPatientDataState, RedcapPatientDataPersistedEvent evento):
                    if (evento.ItemAlreadyExistent)
                    {
                        SetCompleted();
                    }
                    else
                    {
                        if (ContextData.Summary.MetadataFhirId=="")
                        {
                            CurrentState = new SendRedcapMetaDataToFHIRState();
                        }
                        else
                        {
                        CurrentState = new SendRedcapPatientDataToFHIRState();
                        }                        
                    }
                    break;
                case (SendRedcapMetaDataToFHIRState, RedcapStudyMetadataSentEvent evento):
                    if(ContextData.Summary.MetadataFhirId == "")
                    {
                        ContextData.Summary.MetadataFhirId = evento.MetadataIdentifier.ToString();
                    }
                    CurrentState = new SendRedcapPatientDataToFHIRState();
                    break;
                case (SendRedcapPatientDataToFHIRState, RedcapPatientDataSentEvent evento):
                    CurrentState = new UpdateRedcapPatientDataUploadDateState();
                    break;

                case (UpdateRedcapPatientDataUploadDateState, RedcapPatientDataUpdateCompletedEvent evento):
                    SetCompleted();
                    break;



            }
            SetCurrentStateValues();
            return CurrentState;
        }
    }

    public class RedcapStateMachineContext
    {
        public Guid StateMachineID { get; set; }
        public string MasterPatientIndex { get; set; }
        public string FhirPatientId { get; set; }
        public Guid CloudPatientId { get; set; }
        
        public string RedcapStudyDefinition {  get; set; }
        public string RedCapID { get; set; }

        public DateTime StartDate { get; set; }

        public RedcapPatientDataSummary Summary { get; set; }
    }
}

