using System;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class PatientDto : FullAuditedEntityDto
    {
        public Guid CloudPatientId { get; set; }
        public string MasterPatientIndex { get; set; }
        public string FhirPatientId { get; set; }
        public DateTime? LastLaboratoryDataUpdateDate { get; set; }
        public DateTime? LastMedicalDataUpdateDate { get; set; }
        public DateTime? LastDicomDataUpdateDate { get; set; }
        public DateTime? LastClinicalDataUpdateDate { get; set; }
		public DateTime? LastDbUriDataUpdateDate { get; set; }
		public DateTime? LastPathoxDataUpdateDate { get; set; }
        public DateTime? LastIntensiveCareDataUpdateDate { get; set; }
        public DateTime? LastRedcapDataUpdateDate { get; set; }
        public DateTime? LastDeletionRequestDate { get; set; }
    }
}
