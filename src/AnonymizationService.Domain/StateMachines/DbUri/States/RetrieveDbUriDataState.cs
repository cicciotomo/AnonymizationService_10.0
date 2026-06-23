using AnonymizationService.Jobs.RetrieveDbUriData;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.DbUri.States
{
	internal class RetrieveDbUriDataState : State
	{
		public RetrieveDbUriDataState() { }

		protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
		{
			var jobScheduler = serviceProvider.GetService<IJobScheduler>();

			if (stateMachine is not DbUriStateMachine dbUriStateMachine)
			{
				throw new Exception("Invalid state machine for the given state");
			}

			await jobScheduler.EnqueueJob<RetrieveDbUriDataJob, RetrieveDbUriDataArgs, JobResult>(new RetrieveDbUriDataArgs()
			{
				StateMachineId = stateMachine.Id,
				CloudPatientId = dbUriStateMachine.ContextData.CloudPatientId,
				TaxCode = dbUriStateMachine.ContextData.TaxCode,
				MasterPatientIndex = dbUriStateMachine.ContextData.MasterPatientIndex
			});
		}
	}
}
