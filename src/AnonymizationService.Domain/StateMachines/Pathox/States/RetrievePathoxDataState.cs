using AnonymizationService.Jobs.RetrieveDbUriData;
using AnonymizationService.Jobs.RetrievePathoxData;
using AnonymizationService.StateMachines.DbUri;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Pathox.States
{
	public class RetrievePathoxDataState : State
	{
		public RetrievePathoxDataState() { }
		protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
		{
			var jobScheduler = serviceProvider.GetService<IJobScheduler>();

			if (stateMachine is not PathoxStateMachine pathoxStateMachine)
			{
				throw new Exception("Invalid state machine for the given state");
			}

			await jobScheduler.EnqueueJob<RetrievePathoxDataJob, RetrievePathoxDataArgs, JobResult>(new RetrievePathoxDataArgs()
			{
				StateMachineId = stateMachine.Id,
				CloudPatientId = pathoxStateMachine.ContextData.CloudPatientId,
				MasterPatientIndex = pathoxStateMachine.ContextData.MasterPatientIndex,
				StartDate = pathoxStateMachine.ContextData.StartDate
			});
		}
	}
}
