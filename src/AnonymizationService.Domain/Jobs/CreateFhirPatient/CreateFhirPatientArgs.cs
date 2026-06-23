using Hl7.Fhir.Model;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.CreateFhirPatient
{
    internal class CreateFhirPatientArgs : JobArgs
    {
        public Guid CloudPatientId { get; set; }
        public AdministrativeGender Gender { get; set; } 
        public DateTime? BirthDate { get; set; }
        public List<string> FhirStudyIds { get; set; }
    }
}
