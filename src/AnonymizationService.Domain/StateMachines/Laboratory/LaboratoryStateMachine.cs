using AnonymizationService.LaboratoryExams;
using AnonymizationService.StateMachines.Laboratory.States;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.Laboratory
{
    public class LaboratoryStateMachine : ContextStateMachine<LaboratoryStateMachineContext>
    {
        public override StateMachineEvent StateMachineCompletedEventType => new LaboratoryExamsStateMachineCompletedEvent();

        private LaboratoryStateMachine() { }

        internal LaboratoryStateMachine(Guid cloudPatientId, string masterPatientIndex, string fhirPatientId, Guid? parentStateMachineId, DateTime startDate)
        {
            ParentStateMachineId = parentStateMachineId;
            ContextData = new LaboratoryStateMachineContext
            {
                FhirPatientId = fhirPatientId,
                MasterPatientIndex = masterPatientIndex,
                CloudPatientId = cloudPatientId,
                StartDate = startDate
            };
            Initialize();
        }

        public override State Initialize()
        {
            CurrentState = new RetrieveLaboratoryExamsState();
            SetCurrentStateValues();
            return CurrentState;
        }

        protected override State ReactToEvent(Event receivedEvent)
        {
            switch (CurrentState, receivedEvent)
            {
                case (RetrieveLaboratoryExamsState, LaboratoryExamsRetrievedEvent laboratoryExamsRetrievedEvent):
                    this.ContextData.LaboratoryExamsResultList = laboratoryExamsRetrievedEvent.LaboratoryExamsRetrieved;
                    CurrentState = new PersistLaboratoryExamsState();
                    break;
                case (PersistLaboratoryExamsState, LaboratoryExamsPersistedEvent):
                    CurrentState = new UploadDataToFhirServiceState();
                    break;
                case (UploadDataToFhirServiceState, LaboratoryExamsUploadToFhirCompletedEvent laboratoryExamsUploadToFhirCompletedEvent):
                    CurrentState = new UpdateCloudUploadDateForLaboratoryExamsState(laboratoryExamsUploadToFhirCompletedEvent.FhirLaboratoryDataDtos);
                    break;
                case (UpdateCloudUploadDateForLaboratoryExamsState, LaboratoryExamsCheckUpdateCompletedEvent):
                    SetCompleted();
                    break;
            }
            SetCurrentStateValues();
            return CurrentState;
        }
    }

    public class LaboratoryStateMachineContext
    {
        public string FhirPatientId { get; set; }
        public string MasterPatientIndex { get; set; }
        public Guid CloudPatientId { get; set; }
        public DateTime StartDate { get; set; }
        public List<LaboratoryExam> LaboratoryExamsResultList { get; set; }
    }
}
