using AnonymizationService.Jobs.RetrieveDbUriData;
using AnonymizationService.PathoxData;
using AnonymizationService.Services.DbUri;
using AnonymizationService.Services.Pathox;
using AnonymizationService.StateMachines.DbUri;
using AnonymizationService.StateMachines.Pathox;
using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.Jobs.RetrievePathoxData
{
	internal class RetrievePathoxDataJob : Job<RetrievePathoxDataArgs, JobResult>
	{
		private readonly ILogger<RetrievePathoxDataJob> _logger;
		private readonly IPathoxService _pathoxService;
		private readonly IRepository<PathoxExam> _pathoxExamRepository;

		public RetrievePathoxDataJob(ILogger<RetrievePathoxDataJob> logger
			, IPathoxService pathoxService
			, IRepository<PathoxExam> pathoxExamRepository)
		{
			_logger = logger;
			_pathoxService = pathoxService;

			_pathoxExamRepository = pathoxExamRepository;
		}

		public override async Task<JobResult> ExecuteJobAsync(RetrievePathoxDataArgs args)
		{
			_logger.LogInformation($"Retrieving Pathox Data for patient {args.CloudPatientId}");

			var pathoxListResponse = await _pathoxService.GetPatientDataByMasterPatientIndexAsync(args.MasterPatientIndex, args.StartDate);

			var pathoxExamResults = new List<PathoxExamResult>();

			var uploadedPathoxExams = await _pathoxExamRepository.GetListAsync(e => e.CloudPatientId == args.CloudPatientId && e.CloudUploadDate != null);
			var uploadedPathoxExamIds = uploadedPathoxExams.Select(x => x.Id).ToList();

			var examListToUpload = pathoxListResponse.examIdList.Where(id => !uploadedPathoxExamIds.Contains(id)).ToList();
			
			foreach (var exam in examListToUpload)
			{
				var pathoxExam = await _pathoxService.GetExamDetailByExamIdAsync(exam);
				pathoxExamResults.Add(pathoxExam);
			}

			_logger.LogInformation($"Retrieved Pathox Data for patient {args.CloudPatientId}");

			return new JobResult()
			{
				StateMachineId = args.StateMachineId,
				JobResultEvent = new PathoxDataRetrievedEvent(pathoxExamResults)
			};
		}
	}
}
