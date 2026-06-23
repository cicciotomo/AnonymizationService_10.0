using AnonymizationService.ClinicalDocuments;
using AnonymizationService.Services.Dicom;
using AnonymizationService.StateMachines.LoadPatientData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnonymizationService.Services.PatientLoadManager
{
    public class PatientUploadInput
    {
        public string MasterPatientIndex { get; set; }
        public string StudyId { get; set; }

        public bool ClinicalStateMachineShouldStart { get; set; }
        public DateTime? ClinicalStateMachineStartTime { get; set; }

        //public string ClinicalStateMachineDocumentType { get; set; }
        public ClinicalDocumentTypeFilter[] ClinicalStateMachineDocumentType { get; set; }


        public bool DicomStateMachineShouldStart { get; set; }
        public DateTime? DicomStateMachineStartTime { get; set; }
        public DateTime? DicomStateMachineEndTime { get; set; }
        public string[] DicomStateMachineModalities { get; set; }
        public string DicomPacsSource { get; set; }
        public DicomParametersJson DicomParametersJson { get; set; }

        public bool LaboratoryStateMachineShouldStart { get; set; }
        public DateTime? LaboratoryStateMachineStartTime { get; set; }

		public bool DbUriStateMachineShouldStart { get; set; }
		public DateTime? DbUriStateMachineStartTime { get; set; }

		public bool PathoxStateMachineShouldStart { get; set; }
		public DateTime? PathoxStateMachineStartTime { get; set; }

        public bool IntensiveCareStateMachineShouldStart { get; set; }
        public DateTime? IntensiveCareStateMachineStartTime { get; set; }
        public string IntensiveCareStateMachineNosologicalCode { get; set; }

        public bool RedcapStateMachineShouldStart { get; set; }
        public DateTime? RedcapStateMachineStartTime { get; set; }
        public string RedcapStateMachineRedcapStudyConfigurationId { get; set; }
        public string RedcapStateMachineRecordId { get; set; }
    }
}
