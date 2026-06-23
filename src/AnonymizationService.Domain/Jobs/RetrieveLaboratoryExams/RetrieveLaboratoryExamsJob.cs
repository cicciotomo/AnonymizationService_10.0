using System.Linq;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.Services.Galileo;
using AnonymizationService.StateMachines.Laboratory;
using Porini.Abp.StateMachineEngine.Jobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AnonymizationService.Jobs.RetrieveLaboratoryExams
{
    internal class RetrieveLaboratoryExamsJob : Job<RetrieveLaboratoryExamsArgs, JobResult>
    {
        private readonly ILaboratoryExamsService _laboratoryExamService;
        private readonly ILogger<RetrieveLaboratoryExamsJob> _logger;

        public RetrieveLaboratoryExamsJob(ILaboratoryExamsService laboratoryExamService, ILogger<RetrieveLaboratoryExamsJob> logger)
        {
            _laboratoryExamService = laboratoryExamService;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(RetrieveLaboratoryExamsArgs args)
        {
            _logger.LogInformation($"Retrieving laboratory exams for patient {args.CloudPatientId} and start date {args.StartDate}");

            var examResults = await _laboratoryExamService.GetPatientLaboratoryExamResultsAsync(args.MasterPatientIndex, args.StartDate);

            var laboratoryExams = examResults
                .GroupBy(e => (e.ID_VISIT_HOSP))
                .Select(e => new LaboratoryExam(
                    id: e.Key,
                    examStartDate: e.MinBy(exam => exam.DAY).DAY,
                    examEndDate: e.MaxBy(exam => exam.DAY).DAY,
                    cloudPatientId: args.CloudPatientId,
                    valueResults: e.Select(exam => new LaboratoryExamResult(
                        exam.ID_LAB_RESULT.ToString(),
                        exam.RESULT_VALUE,
                        exam.METHOD_ID,
                        exam.REFERENCE_LOW,
                        exam.REFERENCE_HIGH,
                        exam.REFERENCE_RANGE,
                        exam.LAB_COMMENT,
                        exam.UNIT,
                        exam.METHOD_TEXT,
                        exam.DAY,
                        exam.ID_EXAM_FILLER,
                        exam.ID_ORDER_FILLER
                        )).ToList()
                    )).ToList();
            
            _logger.LogInformation($"Retrieved {laboratoryExams.Count} laboratory exams for patient {args.CloudPatientId}");

            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new LaboratoryExamsRetrievedEvent(laboratoryExams)
            };
        }
    }
}
