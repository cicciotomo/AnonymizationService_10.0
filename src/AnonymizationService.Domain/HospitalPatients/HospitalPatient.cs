using AnonymizationService.ClinicalDocuments;
using AnonymizationService.DbUriData;
using AnonymizationService.IntensiveCareData;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.PathoxData;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace AnonymizationService.HospitalPatients
{
    public class HospitalPatient : FullAuditedAggregateRoot
    {
        public Guid CloudPatientId { get; protected set; }
        public string MasterPatientIndex { get; protected set; }
        public string FhirPatientId { get; set; }
        public DateTime? LastLaboratoryDataUpdateDate { get; set; }
        public DateTime? LastClinicalDataUpdateDate { get; set; }
        public DateTime? LastMedicalDataUpdateDate { get; set; }
        public DateTime? LastDicomDataUpdateDate { get; set; }
        public DateTime? LastDbUriDataUpdateDate { get; set; }
		public DateTime? LastPathoxDataUpdateDate { get; set; }
        public DateTime? LastIntensiveCareDataUpdateDate { get; set; }
        public DateTime? LastRedcapDataUpdateDate { get; set; }


        public IEnumerable<LaboratoryExam> LaboratoryExams { get; set; }
        public IEnumerable<ClinicalDocument> ClinicalDocuments { get; set; }
		public IEnumerable<DbUriFUpItem> DbUriFUpItems { get; set; }
		public IEnumerable<DbUriEvent> DbUriEvents { get; set; }
		public IEnumerable<PathoxExam> PathoxExams { get; set; }
        public IEnumerable<IntensiveCarePatientData> IntensiveCarePatientDatas { get; set; }
        //public IEnumerable<Redcap> Redcaps { get; set; }

        public bool DeletionRequested => LastDeletionRequestDate.HasValue;
        public DateTime? LastDeletionRequestDate { get; protected set; }

        protected HospitalPatient() { }

        public HospitalPatient(Guid cloudPatientId, string masterPatientIndex)
        {
            if (string.IsNullOrWhiteSpace(masterPatientIndex))
            {
                throw new ArgumentException($"'{nameof(masterPatientIndex)}' non può essere Null o uno spazio vuoto.", nameof(masterPatientIndex));
            }

            CloudPatientId = cloudPatientId;
            MasterPatientIndex = masterPatientIndex;
        }

        public void RequireDeletion()
        {
            LastDeletionRequestDate = DateTime.Now;
        }

        public override object[] GetKeys()
        {
            return new object[] { CloudPatientId };
        }
    }
}
