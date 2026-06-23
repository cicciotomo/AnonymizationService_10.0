using AnonymizationService.StateMachines.LoadPatientData.States;
using Porini.Abp.StateMachineEngine.Events;
using System;

namespace AnonymizationService.StateMachines.LoadPatientData
{
    public class PatientValidationSucceededEvent : JobEvent 
    { 
        public string TaxCode { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }


        public PatientValidationSucceededEvent(string taxCode, string gender, DateTime? birthDate)
        {
            TaxCode = taxCode;
            Gender = gender;
            BirthDate = birthDate;
        }
    }
    public class PatientValidationFailedEvent : JobEvent { }
    public class PatientValidationFailedHandledEvent : JobEvent
    {
        public Guid StateMachineId { get; }

        public PatientValidationFailedHandledEvent(Guid stateMachineId)
        {
            StateMachineId = stateMachineId;
        }
    }

    public class LoadAllPatientDataEvent : JobEvent
    {
        public LoadPatientMedicalDataStateContent LoadPatientMedicalDataStateContent { get; }
        public LoadAllPatientDataEvent(LoadPatientMedicalDataStateContent loadPatientMedicalDataStateContent)
        {
            LoadPatientMedicalDataStateContent = loadPatientMedicalDataStateContent;
        }
    }

    public class PatientCreatedEvent : JobEvent
    {
        public PatientCreatedEvent(Guid cloudPatientId) { CloudPatientId = cloudPatientId; }
        public Guid CloudPatientId { get; }
    }

    public class PatientDataCheckUpdateCompletedEvent : JobEvent { }

    public class PatientDataLoadStateMachineCompleteEvent : StateMachineEvent { }

    public class FhirPatientCreatedEvent : JobEvent
    {
        public FhirPatientCreatedEvent(string fhirPatientId) { FhirPatientId = fhirPatientId; }
        public string FhirPatientId { get; }
    }
}
