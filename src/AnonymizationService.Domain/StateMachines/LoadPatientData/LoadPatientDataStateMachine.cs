using AnonymizationService.ClinicalDocuments;
using AnonymizationService.Services.Dicom;
using AnonymizationService.StateMachines.LoadPatientData.States;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.LoadPatientData
{
    public class LoadPatientDataStateMachine : ContextStateMachine<LoadPatientDataStateMachineContext>
    {
        public override StateMachineEvent StateMachineCompletedEventType => new PatientDataLoadStateMachineCompleteEvent();

        private LoadPatientDataStateMachine() { }

        public LoadPatientDataStateMachine(Guid id, LoadPatientDataStateMachineParameters parameters) : base(id)
        {
            ContextData = new LoadPatientDataStateMachineContext
            {
                MasterPatientIndex = parameters.MasterPatientIndex,
                FhirStudyId = parameters.StudyId,
                ClinicalParameters = new ClinicalParameters
                {
                    StateMachineShouldStart = parameters.ClinicalStateMachineShouldStart,
                    StartDate = parameters.ClinicalStateMachineStartTime,
                    DocumentType = parameters.ClinicalStateMachineDocumentType
                },

                DicomParameters = new DicomParameters
                {
                    StateMachineShouldStart = parameters.DicomStateMachineShouldStart,
                    StartDate = parameters.DicomStateMachineStartTime,
                    EndDate = parameters.DicomStateMachineEndTime,
                    DicomModalities = parameters.DicomStateMachineModalities,
                    PacsSource = parameters.DicomStateMachinePacsSource,
                    DicomParametersJson = parameters.DicomStateMachineParametersJson
                },

                IntensiveCareParameters = new IntesiveCareParameters
                { 
                    StateMachineShouldStart = parameters.IntensiveCareStateMachineShouldStart,
                    StartDate = parameters.IntensiveCareStateMachineStartTime,
                    NosologicalCodes = parameters.IntensiveCareStateMachineNosologicalCodes
                },

                RedcapParameters = new RedcapParameters
                {
                    StateMachineShouldStart = parameters.RedcapStateMachineShouldStart,
                    StartDate = parameters.RedcapStateMachineStartTime,
                    RedcapStudyConfigurationId = parameters.RedcapStateMachineRedcapStudyConfigurationId,
                    RecordId = parameters.RedcapStateMachineRecordId
                },

                LaboratoryParameters = new LaboratoryParameters
                {
                    StateMachineShouldStart = parameters.LaboratoryStateMachineShouldStart,
                    StartDate = parameters.LaboratoryStateMachineStartTime
                },

				DbUriParameters = new DbUriParameters
				{
					StateMachineShouldStart = parameters.DbUriStateMachineShouldStart,
					StartDate = parameters.DbUriStateMachineStartTime
				},

				PathoxParameters = new PathoxParameters
				{
					StateMachineShouldStart = parameters.PathoxStateMachineShouldStart,
					StartDate = parameters.PathoxStateMachineStartTime
				}
			};
            Initialize();
        }

        public override State Initialize()
        {
            CurrentState = new ValidatePatientState();
            SetCurrentStateValues();
            return CurrentState;
        }

        protected override State ReactToEvent(Event eventReceived)
        {
            switch (CurrentState, eventReceived)
            {
                case (ValidatePatientState, PatientValidationSucceededEvent patientValidationSucceededEvent):
                    ContextData.TaxCode = patientValidationSucceededEvent.TaxCode;
                    ContextData.Gender = patientValidationSucceededEvent.Gender;
                    ContextData.Birthdate = patientValidationSucceededEvent.BirthDate;
					CurrentState = new CreatePatientState();
                    break;
                case (ValidatePatientState, PatientValidationFailedEvent):
                    CurrentState = new PatientValidationFailedState();
                    break;
                case (PatientValidationFailedState, PatientValidationFailedHandledEvent):
                    SetCompleted();
                    break;
                case (CreatePatientState, PatientCreatedEvent patientCreatedEvent):
                    ContextData.CloudPatientId = patientCreatedEvent.CloudPatientId;
                    CurrentState = new CreateFhirPatientState();
                    break;
                case (CreateFhirPatientState, FhirPatientCreatedEvent fhirPatientCreatedEvent):
                    ContextData.FhirPatientId = fhirPatientCreatedEvent.FhirPatientId;
                    CurrentState = new LoadPatientMedicalDataState();
                    break;
                case (LoadPatientMedicalDataState, LoadAllPatientDataEvent loadAllPatientDataEvent):
                    CurrentState = new WaitChildrenStateMachinesCompletionState(loadAllPatientDataEvent.LoadPatientMedicalDataStateContent);
                    break;
                case (WaitChildrenStateMachinesCompletionState, _):
                    CurrentState.HandleEvent(eventReceived);
                    if (CurrentState.CanGoNext())
                    {
                        CurrentState = new UpdatePatientUploadedState();
                    }
                    break;
                case (UpdatePatientUploadedState, PatientDataCheckUpdateCompletedEvent):
                    SetCompleted();
                    break;
            }
            SetCurrentStateValues();
            return CurrentState;
        }
    }

    public class LoadPatientDataStateMachineContext
    {
        public string FhirStudyId { get; set; }
        public string FhirPatientId { get; set; }
        public Guid? CloudPatientId { get; set; }
        public string MasterPatientIndex { get; set; }
        public string TaxCode { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public ClinicalParameters ClinicalParameters { get; set; }
        public DicomParameters DicomParameters { get; set; }
        public LaboratoryParameters LaboratoryParameters { get; set; }
        public DbUriParameters DbUriParameters { get; set; }
        public PathoxParameters PathoxParameters { get; set; }
        public IntesiveCareParameters IntensiveCareParameters { get; set; }
        public RedcapParameters RedcapParameters { get; set; }

    }

    public class LaboratoryParameters
    {
        public bool StateMachineShouldStart { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class DicomParameters
    {
        public bool StateMachineShouldStart { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string[] DicomModalities { get; set; }
        public string PacsSource { get; set; }
        public DicomParametersJson DicomParametersJson { get; set; }
    }

    //public class DicomParametersJson
    //{
    //    public bool MoveToPacsRicerca { get; set; }
    //    public string StudyDescription { get; set; }
    //}

    public class IntesiveCareParameters
    {
        public bool StateMachineShouldStart { get; set; }
        public DateTime? StartDate { get; set; }
        public IEnumerable<string> NosologicalCodes { get; set; }
    }

    public class RedcapParameters
    {
        public bool StateMachineShouldStart { get; set; }
        public DateTime? StartDate { get; set; }
        public string RedcapStudyConfigurationId { get; set; }
        public string RecordId { get; set; }
    }

    public class ClinicalParameters
    {
        public bool StateMachineShouldStart { get; set; }
        public DateTime? StartDate { get; set; }
        public ClinicalDocumentTypeFilter[] DocumentType { get; set; }
    }
	public class DbUriParameters
	{
		public bool StateMachineShouldStart { get; set; }
		public DateTime? StartDate { get; set; }
	}
	public class PathoxParameters
	{
		public bool StateMachineShouldStart { get; set; }
		public DateTime? StartDate { get; set; }
	}
}
