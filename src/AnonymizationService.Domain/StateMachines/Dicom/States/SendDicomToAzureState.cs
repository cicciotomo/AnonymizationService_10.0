using AnonymizationService.DicomData;
using AnonymizationService.Jobs.SendDicomToAzure;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Dicom.States
{
    internal class SendDicomToAzureState : StatefulState<SendDicomToAzureStateContent>
    {
        public SendDicomToAzureState() { }
        public SendDicomToAzureState(List<DicomStudy> dicomStudies)
        {
            this.StateData.DicomStudies = dicomStudies;
        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not DicomStateMachine dicomStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var serieToSendParameters = new List<SerieToSendParameter>();

            foreach (var study in this.StateData.DicomStudies)
            {
                var seriesToUploadIds = study.DicomSeries.Select(se => se.Id).ToList();
                serieToSendParameters.AddRange(seriesToUploadIds.Select(i => new SerieToSendParameter { SerieId = i, StudyFilePath = study.StudyFilePath, StudyId = study.Id }).ToList());
            }

            await JobScheduler.EnqueueJob<SendDicomToAzureJob, SendDicomToAzureArgs, JobResult>(new SendDicomToAzureArgs()
            {
                PatientId = dicomStateMachine.ContextData.CloudPatientId,
                SerieToSendParameters = serieToSendParameters,
                StateMachineId = stateMachine.Id
            });
        }
    }

    public class SendDicomToAzureStateContent
    {
        public List<DicomStudy> DicomStudies { get; set; }
    }

    public class SerieToSendParameter
    {
        public string StudyId { get; set; }
        public string SerieId { get; set; }
        public string StudyFilePath { get; set; }
    }
}
