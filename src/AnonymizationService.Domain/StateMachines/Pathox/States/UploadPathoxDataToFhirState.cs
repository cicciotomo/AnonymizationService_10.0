using AnonymizationService.DbUriData;
using AnonymizationService.Jobs.UpsertFhirPathoxData;
using AnonymizationService.PathoxData;
using AnonymizationService.StateMachines.DbUri;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Pathox.States
{
	public class UploadPathoxDataToFhirState : State
	{
		public UploadPathoxDataToFhirState() { }
		protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
		{
			var jobScheduler = serviceProvider.GetService<IJobScheduler>();

			if (stateMachine is not PathoxStateMachine pathoxStateMachine)
			{
				throw new Exception("Invalid state machine for the given state");
			}

			var pathoxFUpItemsRepository = serviceProvider.GetService<IRepository<PathoxExam>>();

			var retrivedPathoxExamIds = pathoxStateMachine.ContextData.PathoxExams.Select(i => i.exam.groupNumberAlt).ToList();

			var pathoxFUpItemsToUpload = await pathoxFUpItemsRepository.GetListAsync(e => retrivedPathoxExamIds.Contains(e.Id) && e.CloudUploadDate == null);
			var pathoxFUpItemToUploadIds = pathoxFUpItemsToUpload.Select(i => i.Id).ToList();


			await jobScheduler.EnqueueJob<UpsertFhirPathoxDataJob, UpsertFhirPathoxDataArgs, JobResult>(new UpsertFhirPathoxDataArgs()
			{
				StateMachineId = stateMachine.Id,
				FhirPatientId = pathoxStateMachine.ContextData.FhirPatientId,
				CloudPatientId = pathoxStateMachine.ContextData.CloudPatientId,
				PathoxExamResults = pathoxStateMachine.ContextData.PathoxExams.Where(i => pathoxFUpItemToUploadIds.Contains(i.exam.groupNumberAlt)).ToList()
			});
		}
	}
}
