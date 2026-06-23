using AnonymizationService.ClinicalDocuments;
using System;

namespace AnonymizationService.Services.FhirResourcesFilterService
{
    public class FilterManipulationData
    {
        public string FhirPatientId { get; set; }
        public Guid PatientIdentifier { get; set; }
        public DateTime EncounterEndDate { get; set; }
        public int EncounterId { get; set; }
        public string DocumentName { get; set; }
        public string DepartmentIdentifier { get; set; }

        public ClinicalDocumentType DocumentType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
