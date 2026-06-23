using AnonymizationService.ClinicalDocumentTypes;
using AnonymizationService.Enums;
using AnonymizationService.StateMachines.Dicom;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class UploadRequestDto : FullAuditedEntityDto<Guid>
    {
        public string MasterPatientIndex { get; set; }
        public List<string> StudyIds { get; set; }
        public bool ClinicalStateMachineShouldStart { get; set; }
        public DateTime? ClinicalStateMachineStartTime { get; set; }
        public ClinicalDocumentTypeFilterDto[] ClinicalStateMachineDocumentType { get; set; }
        public bool DicomStateMachineShouldStart { get; set; }
        public DateTime? DicomStateMachineStartTime { get; set; }
        public DateTime? DicomStateMachineEndTime { get; set; }
        public string DicomStateMachineModalities { get; set; }

        public bool DicomStateMachineMoveToPacsRicerca { get; set; }
        public string DicomStateMachineStudyDescription { get; set; }   
        public string PacsSource { get; set; }
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
        public Guid? BatchRequestId { get; set; }
        public PatientUploadStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public string IntensiveCareFailedPipelineRunID { get; set; }
        public bool FlagIntensiveCareFailedPipelineRun { get; set; }
        public Guid StateMachineId { get; set; }
    }
}
