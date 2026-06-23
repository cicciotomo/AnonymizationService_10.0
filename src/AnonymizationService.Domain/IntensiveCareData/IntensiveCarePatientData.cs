using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.IntensiveCareData
{
    public class IntensiveCarePatientData : Entity<int>
    {
        public IntensiveCarePatientData(Guid cloudPatientId, string nosologicalCode, string patientId, DateTimeOffset examStartDate, DateTimeOffset? examEndDate, Guid stateMachineId) 
        {
            CloudPatientId = cloudPatientId;
            PatientId = patientId;
            ExamStartDate = examStartDate;
            ExamEndDate = examEndDate;
            NosologicalCode = nosologicalCode;
            StateMachineId = stateMachineId;
        }

        public Guid CloudPatientId {  get; set; }
        public DateTime? CloudUploadDate { get; set; }
        public string NosologicalCode { get; set; }
        public string PatientId { get; set; }
        public DateTimeOffset ExamStartDate { get; set; }
        public DateTimeOffset? ExamEndDate { get; set; }
        public Guid? PipelineId { get; set; }
        public DateTime? PipelineRunCreationTime { get; set; }
        public DateTime? PipelineRunEndTime { get; set; }
        public Boolean? PipelineRunResult { get; set; }
        public string PipelineRunMessage { get; set; }
        public Guid StateMachineId { get; set; }

        public void SetCloudUploadDate(DateTime uploadDate)
        { 
            CloudUploadDate = uploadDate;
        }
    }
}
