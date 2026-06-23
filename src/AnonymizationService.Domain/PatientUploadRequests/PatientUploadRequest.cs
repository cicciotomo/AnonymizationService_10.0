using AnonymizationService.ClinicalDocuments;
using AnonymizationService.Enums;
using AnonymizationService.StateMachines.LoadPatientData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace AnonymizationService.PatientUploadRequests
{
    public class PatientUploadRequest : CreationAuditedEntity<Guid>
    {
        private PatientUploadRequest() { }
        public string MasterPatientIndex { get; protected set; }
        public string StudyId { get; protected set; }

        public bool ClinicalStateMachineShouldStart { get; set; }
        public DateTime? ClinicalStateMachineStartTime { get; set; }
        public ClinicalDocumentTypeFilter[] ClinicalStateMachineDocumentType { get; set; }

        public bool DicomStateMachineShouldStart { get; set; }
        public DateTime? DicomStateMachineStartTime { get; set; }
        public DateTime? DicomStateMachineEndTime { get; set; }
        public string DicomStateMachineModalities { get; set; }
        public string DicomPacsSource { get; set; }
        public string DicomStateMachineParameterJson { get; set; }

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

        public Guid? BatchRequestId { get; protected set; }
        public PatientUploadStatus Status { get; protected set; }
        public string ErrorMessage { get; protected set; }

        public bool ClinicalStartTimeIsDefault { get; set; }
        public bool DicomStartTimeIsDefault { get; set; }
        public bool LaboratoryStartTimeIsDefault { get; set; }
        public bool DbUriStartTimeIsDefault { get; set; }
        public bool PathoxStartTimeIsDefault { get; set; }
        public bool IntensiveCareStartTimeIsDefault { get; set; }
        public bool RedcapStartTimeIsDefault { get; set; }

        public bool DicomModalitiesAreDefault { get; set; }
        public bool ClinicalDocumentTypeAreDefault { get; set; }

        public Guid StateMachineId { get; protected set; }


        public PatientUploadRequest(Guid id
            , string masterPatientIndex
            , string studyId
            , bool clinicalStateMachineShouldStart
            , DateTime? clinicalStateMachineStartTime
            , bool dicomStateMachineShouldStart
            , DateTime? dicomStateMachineStartTime
            , DateTime? dicomStateMachineEndTime
            , string dicomStateMachineModalities
            , string dicomStateMachinePacsSource
            , string dicomStateMachineParametersJson
            , bool intensiveCareStateMachineShouldStart    
            , DateTime? intensiveCareStateMachineStartTime
            , string intensiveCareStateMachineNosologicalCode
            , bool redcapStateMachineShouldStart
            , DateTime? redcapStateMachineStartTime
            , string redcapStateMachineRedcapStudyConfigurationId
            , string redcapStateMachineRecordId
            , bool laboratoryStateMachineShouldStart
            , DateTime? laboratoryStateMachineStartTime
            , Guid userId
            , ClinicalDocumentTypeFilter[] clinicalStateMachineDocumentType
            , bool clinicalStartTimeIsDefault
            , bool dicomStartTimeIsDefault
            , bool laboratoryStartTimeIsDefault
            , bool intensiveCareStartTimeIsDefault
            , bool redcapCareStartTimeIsDefault
            , bool dicomModalitiesAreDefault
            , bool clinicalDocumentTypeAreDefault
            , Guid statemachineId
            , bool dbUriStateMachineShouldStart
            , DateTime? dbUriStateMachineStartTime
            , bool dbUriStartTimeIsDefault
            , bool pathoxStateMachineShouldStart
            , DateTime? pathoxStateMachineStartTime
            , bool pathoxStartTimeIsDefault

            , Guid? batchRequestId = null

            )
        {
            Id = id;
            MasterPatientIndex = masterPatientIndex;
            StudyId = studyId;

            CreatorId = userId;
            Status = PatientUploadStatus.Pending;

            StateMachineId = statemachineId;
            BatchRequestId = batchRequestId;

            ClinicalStateMachineShouldStart = clinicalStateMachineShouldStart;
            ClinicalStateMachineStartTime = clinicalStateMachineStartTime;
            ClinicalStateMachineDocumentType = clinicalStateMachineDocumentType;
            ClinicalStartTimeIsDefault = clinicalStartTimeIsDefault;
            ClinicalDocumentTypeAreDefault = clinicalDocumentTypeAreDefault;

            DicomStateMachineShouldStart = dicomStateMachineShouldStart;
            DicomPacsSource = dicomStateMachinePacsSource;
            DicomStateMachineParameterJson = dicomStateMachineParametersJson;
            DicomStateMachineStartTime = dicomStateMachineStartTime;
            DicomStateMachineEndTime = dicomStateMachineEndTime;
            DicomStateMachineModalities = dicomStateMachineModalities;
            DicomStartTimeIsDefault = dicomStartTimeIsDefault;
            DicomModalitiesAreDefault = dicomModalitiesAreDefault;

            LaboratoryStateMachineShouldStart = laboratoryStateMachineShouldStart;
            LaboratoryStateMachineStartTime = laboratoryStateMachineStartTime;
            LaboratoryStartTimeIsDefault = laboratoryStartTimeIsDefault;

            DbUriStateMachineShouldStart = dbUriStateMachineShouldStart;
            DbUriStateMachineStartTime = dbUriStateMachineStartTime;
            DbUriStartTimeIsDefault = dbUriStartTimeIsDefault;

            PathoxStateMachineShouldStart = pathoxStateMachineShouldStart;
            PathoxStateMachineStartTime = pathoxStateMachineStartTime;
            PathoxStartTimeIsDefault = pathoxStartTimeIsDefault;

            IntensiveCareStateMachineShouldStart = intensiveCareStateMachineShouldStart;
            IntensiveCareStateMachineStartTime = intensiveCareStateMachineStartTime;
            IntensiveCareStateMachineNosologicalCode = intensiveCareStateMachineNosologicalCode;
            IntensiveCareStartTimeIsDefault = intensiveCareStartTimeIsDefault;

            RedcapStateMachineShouldStart = redcapStateMachineShouldStart;
            RedcapStateMachineStartTime = redcapStateMachineStartTime;
            RedcapStartTimeIsDefault = redcapCareStartTimeIsDefault;
            RedcapStateMachineRedcapStudyConfigurationId = redcapStateMachineRedcapStudyConfigurationId;
            RedcapStateMachineRecordId = redcapStateMachineRecordId;
        }

        public void SetFailure(string failureMessage)
        {
            ErrorMessage = failureMessage;
            Status = PatientUploadStatus.Failed;
        }

        public void SetSuccess()
        {
            Status = PatientUploadStatus.Success;
        }

        public void SetRunning()
        {
            Status = PatientUploadStatus.Running;
        }
    }
}
