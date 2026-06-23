using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.LaboratoryExams
{
    public class LaboratoryExamResult : Entity<string>
    {
        public string Value { get; protected set; }
        public string ExamType { get; protected set; }
        public string ExamTypeDescription { get; set; }
        public string ReferenceLow { get; protected set; }
        public string ReferenceHigh { get; protected set; }
        public string ReferenceRange { get; protected set; }
        public string Comment { get; protected set; }
        public string Unit { get; protected set; }
        public DateTime? ResultDate { get; protected set; }
        
        public string ExamMethod { get; set; }
        public string ExamMethodDescription { get; set; }

        public LaboratoryExamResult(string id, string value, string examType, string referenceLow, string referenceHigh, string referenceRange, string comment, string unit, string examTypeDescription, DateTime? resultDate, string examMethod, string examMethodDescription) : base(id)
        {
            Value = value;
            ExamType = examType;
            ReferenceLow = referenceLow;
            ReferenceHigh = referenceHigh;
            ReferenceRange = referenceRange;
            Comment = comment;
            Unit = unit;
            ExamTypeDescription = examTypeDescription;
            ResultDate = resultDate;
            ExamMethod = examMethod;
            ExamMethodDescription = examMethodDescription;
        }

        private LaboratoryExamResult() { }
    }
}
