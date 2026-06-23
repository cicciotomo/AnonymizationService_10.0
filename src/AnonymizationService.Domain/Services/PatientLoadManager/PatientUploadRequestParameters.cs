using AnonymizationService.ClinicalDocuments;
using AnonymizationService.Services.Dicom;
using AnonymizationService.StateMachines.LoadPatientData;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Services.PatientLoadManager
{
    internal class PatientUploadRequestParameters
    {
        internal bool ClinicalStateMachineShouldStart { get; set; }
        internal bool DicomStateMachineShouldStart { get; set; }
        internal bool LaboratoryStateMachineShouldStart { get; set; }
		internal bool DbUriStateMachineShouldStart { get; set; }
		internal bool PathoxStateMachineShouldStart { get; set; }
        internal bool IntensiveCareStateMachineShouldStart { get; set; }
        internal bool RedcapStateMachineShouldStart { get; set; }

        internal string MasterPatientIndex { get; set; }
        internal string StudyId { get; set; }


        internal DateTime? ClinicalStateMachineStartTime { get; set; }
        internal ClinicalDocumentTypeFilter[] ClinicalStateMachineDocumentType { get; set; }

        internal DateTime? DicomStateMachineStartTime { get; set; }
        internal string DicomStateMachinePacsSource { get; set; }
        internal DateTime? DicomStateMachineEndTime { get; set; }
        internal string DicomStateMachineModalities { get; set; }
        internal DicomParametersJson DicomStateMachineParametersJson { get; set; }

        internal DateTime? LaboratoryStateMachineStartTime { get; set; }
		internal DateTime? DbUriStateMachineStartTime { get; set; }
		internal DateTime? PathoxStateMachineStartTime { get; set; }
        internal DateTime? IntensiveCareStateMachineStartTime { get; set; }
        internal DateTime? RedcapStateMachineStartTime { get; set; }

        internal string IntensiveCareStateMachineNosologicalCode { get; set; }

        internal string RedcapStateMachineRedcapStudyConfigurationId { get; set; }
        internal string RedcapStateMachineRecordId { get; set; }

        internal bool UseDefaultClinicalStartTime { get; set; }
        internal bool UseDefaultClinicalDocumentType { get; set; }

        internal bool UseDefaultDicomStartTime { get; set; }
        internal bool UseDefaultDicomModalities { get; set; }

        internal bool UseDefaultLaboratoryStartTime { get; set; }

		internal bool UseDefaultDbUriStartTime { get; set; }
		internal bool UseDefaultPathoxStartTime { get; set; }
        internal bool UseDefaultIntensiveCareStartTime { get; set; }
        internal bool UseDefaultRedcapStartTime { get; set; }
    }
}
