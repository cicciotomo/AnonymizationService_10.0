using AnonymizationService.ClinicalDocuments;
using AnonymizationService.Services.Dicom;
using System;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.LoadPatientData
{
    public class LoadPatientDataStateMachineParameters
    {
        public string MasterPatientIndex { get; set; }
        public string StudyId { get; set; }

        public bool ClinicalStateMachineShouldStart { get; set; }
        public DateTime? ClinicalStateMachineStartTime { get; set; }
        public ClinicalDocumentTypeFilter[] ClinicalStateMachineDocumentType { get; set; }

        public bool DicomStateMachineShouldStart { get; set; }
        public DateTime? DicomStateMachineStartTime { get; set; }
        public DateTime? DicomStateMachineEndTime { get; set; }
        public string[] DicomStateMachineModalities { get; set; }
        public string DicomStateMachinePacsSource { get; set; }
        public DicomParametersJson DicomStateMachineParametersJson { get; set; }
        public bool LaboratoryStateMachineShouldStart { get; set; }
        public DateTime? LaboratoryStateMachineStartTime { get; set; }

		public bool DbUriStateMachineShouldStart { get; set; }
		public DateTime? DbUriStateMachineStartTime { get; set; }

		public bool PathoxStateMachineShouldStart { get; set; }
		public DateTime? PathoxStateMachineStartTime { get; set; }

        public bool IntensiveCareStateMachineShouldStart { get; set; }
        public DateTime? IntensiveCareStateMachineStartTime { get; set; }
        public IEnumerable<string> IntensiveCareStateMachineNosologicalCodes { get; set; }

        public bool RedcapStateMachineShouldStart { get; set; }
        public DateTime? RedcapStateMachineStartTime { get; set; }
        public string RedcapStateMachineRedcapStudyConfigurationId { get; set; }
        public string RedcapStateMachineRecordId { get; set; }
    }
}
