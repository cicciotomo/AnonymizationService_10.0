using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.LaboratoryExams
{
    public class LaboratoryExam : Entity<string>
    {
        public DateTime ExamStartDate { get; protected set; }
        public DateTime ExamEndDate { get; protected set; }
        public List<LaboratoryExamResult> ValueResults { get; protected set; }
        public Guid CloudPatientId { get; protected set; }
        public DateTime? CloudUploadDate { get; protected set; }

        public LaboratoryExam(string id, DateTime examStartDate, DateTime examEndDate, Guid cloudPatientId, List<LaboratoryExamResult> valueResults) : base(id)
        {
            ExamStartDate = examStartDate;
            ExamEndDate = examEndDate;
            ValueResults = valueResults;
            CloudPatientId = cloudPatientId;
        }

        private LaboratoryExam() { }

        public void SetCloudUploadDate(DateTime uploadDate)
        {
            CloudUploadDate = uploadDate;
        }
    }
}
