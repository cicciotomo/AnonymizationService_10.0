using AnonymizationService.DbUriData;
using AnonymizationService.DicomData;
using AnonymizationService.PathoxData;
using AnonymizationService.StateMachines.DbUri;
using AnonymizationService.StateMachines.Dicom;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Pathox.States
{
	public class PersistPathoxDataState : State
	{
		public PersistPathoxDataState() { }
		protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
		{
			if (stateMachine is not PathoxStateMachine pathoxlStateMachine)
			{
				throw new Exception("Invalid state machine for the given state");
			}

			var retrievedPathoxExams = pathoxlStateMachine.ContextData.PathoxExams;

			if (retrievedPathoxExams != null)
			{
				var pathoxExamRepository = serviceProvider.GetService<IRepository<PathoxExam>>();

				var persistedPathoxExams = await pathoxExamRepository.GetListAsync(e => e.CloudPatientId == pathoxlStateMachine.ContextData.CloudPatientId);
				var persistedPathoxExamIds = persistedPathoxExams.Select(e => e.Id).ToList();

				var pathoxExamsToInsert = retrievedPathoxExams
					.Where(e => !persistedPathoxExamIds.Contains(e.exam.groupNumberAlt))
					.Select(e => new PathoxExam(e.exam.groupNumberAlt, pathoxlStateMachine.ContextData.CloudPatientId));

				await pathoxExamRepository.InsertManyAsync(pathoxExamsToInsert);
			}

			await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new PersistedPathoxDataStateEvent()));
		}
	}
}
