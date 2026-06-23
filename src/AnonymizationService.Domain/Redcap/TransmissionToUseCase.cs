using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.Redcap
{
    public class TransmissionToUseCase: Entity<Guid>
    {
        public Guid RedcapStudyConfigurationId { get; private set; }
        public string Data {  get; private set; }
        public DateTime CreationDate { get; private set; }
        public Guid UseCaseId { get; private set; }
        public Guid? PipelineRunId { get; private set; }
        public Boolean? PipelineRunResult { get; set; }
        public string? PipelineRunMessage{ get; private set; }
        public DateTime? PipelineRunCreationTime { get; set; }
        public DateTime? PipelineRunEndTime { get; set; }

        private TransmissionToUseCase() { }
        public TransmissionToUseCase(Guid id,
                                     Guid redcap_study_config_id,
                                     string data,
                                     Guid use_case_id): base(id)
        {
            RedcapStudyConfigurationId = redcap_study_config_id;
            Data = data;
            CreationDate = DateTime.Now;
            UseCaseId = use_case_id;
        }

        public void UpdatePipelineRunID(Guid pipeline_run_id)
        {
            PipelineRunId = pipeline_run_id;
        }

        public void UpdatePipelineRunCreationTime(DateTime date)
        {
            PipelineRunCreationTime = date;
        }

        //public void UpdatePipelineRunStatus(string pipeline_run_status, string pipeline_run_message)
        //{
        //    PipelineRunResult = pipeline_run_status;
        //    PipelineRunMessage = pipeline_run_message;
        //}
    }
}
