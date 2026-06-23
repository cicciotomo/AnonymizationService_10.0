using AnonymizationService.Jobs.RetrieveDicomData;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Dicom.States
{
    internal class RetrieveDicomDataState : State
    {
        public RetrieveDicomDataState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not DicomStateMachine dicomStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            await JobScheduler.EnqueueJob<RetrieveDicomDataJob, RetrieveDicomDataArgs, JobResult>(new RetrieveDicomDataArgs()
            {

                MasterPatientIndex = dicomStateMachine.ContextData.MasterPatientIndex
                , Modalities = dicomStateMachine.ContextData.Modalities
                , CloudPatientId = dicomStateMachine.ContextData.CloudPatientId
                , StateMachineId = stateMachine.Id
                , StartDate = dicomStateMachine.ContextData.StartDate
                , EndDate = dicomStateMachine.ContextData.EndDate
                , PacsSource = dicomStateMachine.ContextData.PacsSource
                , DicomParametersJson = dicomStateMachine.ContextData.DicomParametersJson
            });
        }
    }
}
