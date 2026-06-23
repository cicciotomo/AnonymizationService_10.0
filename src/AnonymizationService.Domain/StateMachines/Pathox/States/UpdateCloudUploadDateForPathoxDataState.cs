using AnonymizationService.HospitalPatients;
using AnonymizationService.PathoxData;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Pathox.States
{
	public class UpdateCloudUploadDateForPathoxDataState : StatefulState<UploadedToFhirPathoxData>
	{
		public UpdateCloudUploadDateForPathoxDataState() { }
		public UpdateCloudUploadDateForPathoxDataState(List<string> uploadedPathoxExamIds) 
		{
			this.StateData = new UploadedToFhirPathoxData { UploadedPathoxExamIds = uploadedPathoxExamIds };
		}
		protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
		{
			if (stateMachine is not PathoxStateMachine pathoxStateMachine)
			{
				throw new Exception("Invalid state machine for the given state");
			}

			var cloudUploadDate = DateTime.Now;


			if (this.StateData.UploadedPathoxExamIds?.Count > 0)
			{
				var uploadedExamIds = this.StateData.UploadedPathoxExamIds;

				var pathoxExamRepository = serviceProvider.GetService<IRepository<PathoxExam>>();

				var uploadedExams = await pathoxExamRepository.GetListAsync(e => e.CloudPatientId == pathoxStateMachine.ContextData.CloudPatientId && uploadedExamIds.Contains(e.Id));
				uploadedExams.ForEach(x => x.SetCloudUploadDate(cloudUploadDate));

				await pathoxExamRepository.UpdateManyAsync(uploadedExams);
			}

			var patientService = serviceProvider.GetService<IHospitalPatientService>();
			await patientService.UpdatePathoxLastUpdateDateForPatientAsync(pathoxStateMachine.ContextData.CloudPatientId, cloudUploadDate);

			await PublishEventOnBusAsync(new ExecutedTaskEvent(pathoxStateMachine.Id, new UpdatedCloudUploadDateForPathoxDataEvent()));

		}

	}
	public class UploadedToFhirPathoxData
	{
		public List<string> UploadedPathoxExamIds { get; set; }
	}
}
