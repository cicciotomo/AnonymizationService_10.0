using AnonymizationService.DicomData;
using AnonymizationService.Services.DicomAnonymizerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Dicom.States
{
    internal class AnonymizeImagesState : State
    {
        public AnonymizeImagesState() { }
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not DicomStateMachine dicomStateMachine)
                {
                    throw new Exception("Invalid state machine for the given state");
                }

                var dicomImageAnonymizer = serviceProvider.GetService<IDicomAnonymizerService>();
                var dicomStudyRepository = serviceProvider.GetService<IDicomStudyRepository>();

                var studiesToAnonymize = new List<DicomStudy>();

                foreach (var study in dicomStateMachine.ContextData.DicomStudies)
                {
                    var existingStudy = await dicomStudyRepository.GetStudyWithSeriesAsync(study.Id);
                    var uploadedSeriesIds = existingStudy.DicomSeries.Where(se => se.CloudUploadDate != null).Select(se => se.Id).ToList();

                    var seriesToAnonymize = study.DicomSeries.Where(se => !uploadedSeriesIds.Contains(se.Id)).ToList();
                    if (seriesToAnonymize.Any())
                    {
                        var studyToAnonymize = new DicomStudy(study.Id, study.CloudPatientId, study.StudyDate, study.StudyFilePath);
                        studyToAnonymize.AddDicomSeries(seriesToAnonymize);

                        studiesToAnonymize.Add(studyToAnonymize);
                    }
                }

                var anonymizationParameters = new AnonymizationParameters()
                {
                    CloudPatientId = dicomStateMachine.ContextData.CloudPatientId
                };

                foreach (var study in studiesToAnonymize)
                {
                    foreach (var serie in study.DicomSeries)
                    {
                        var serieFilePath = Path.Combine(study.StudyFilePath, serie.Id);
                        var outputFilePath = Path.Combine(study.StudyFilePath, "Anonymized", serie.Id);
                        await dicomImageAnonymizer.AnonymizeDicomImagesInFolderAsync(serieFilePath, outputFilePath, anonymizationParameters);
                        Directory.Delete(serieFilePath, true);
                    }
                }

                await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new DicomDataAnonymizedEvent(studiesToAnonymize)));
        }
    }
}
