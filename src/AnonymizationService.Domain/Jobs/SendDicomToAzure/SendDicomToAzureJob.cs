using AnonymizationService.DicomData;
using AnonymizationService.Services.DicomWeb;
using AnonymizationService.StateMachines.Dicom;
using FellowOakDicom;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;
using AnonymizationService.Services.DataPlatform;

namespace AnonymizationService.Jobs.SendDicomToAzure
{
    internal class SendDicomToAzureJob : Job<SendDicomToAzureArgs, JobResult>
    {
        private readonly IDicomWebServiceClient _dicomWebServiceClient;
        private readonly IRepository<DicomSerie, string> _dicomSerieRepository;
        private readonly ILogger<SendDicomToAzureJob> _logger;
        private readonly IDataPlatformService _dataPlatformService;

        public SendDicomToAzureJob(IDicomWebServiceClient dicomWebServiceClient, IRepository<DicomSerie, string> dicomSerieRepository, ILogger<SendDicomToAzureJob> logger, IDataPlatformService dataPlatformService)
        {
            _dicomWebServiceClient = dicomWebServiceClient;
            _dicomSerieRepository = dicomSerieRepository;
            _logger = logger;
            _dataPlatformService = dataPlatformService;
        }

        public override async Task<JobResult> ExecuteJobAsync(SendDicomToAzureArgs args)
        {
            _logger.LogInformation($"Sending Dicom data to Azure DICOM Service for patient {args.PatientId}");

            var cloudUploadDate = DateTime.UtcNow;

            var seriesToSendParametersGroupedByStudy = args.SerieToSendParameters.GroupBy(s => s.StudyFilePath).ToList();

            var uploadDate = DateTime.Now;

            foreach (var serieToSendParametersGroupedByStudy in seriesToSendParametersGroupedByStudy)
            {
                var studyFilePath = serieToSendParametersGroupedByStudy.Key;

                foreach (var serieId in serieToSendParametersGroupedByStudy.Select(s => s.SerieId).ToList())
                {
                    var dicomFiles = new List<DicomFile>();

                    var anonymizedSerieFilePath = Path.Combine(studyFilePath, "Anonymized", serieId);

                    var uploadedData = new List<DicomInstanceMetadata>();
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(anonymizedSerieFilePath, "*.dcm"))
                        {
                            var dicomFile = await DicomFile.OpenAsync(file);
                            dicomFiles.Add(dicomFile);
                            var anonymizedStudyId = dicomFile.Dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
                            var anonymizedSerieId = dicomFile.Dataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
                            var anonymizedInstanceId = dicomFile.Dataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
                            uploadedData.Add(new DicomInstanceMetadata() { SerieUid = anonymizedSerieId, SopInstanceUid = anonymizedInstanceId, StudyUid = anonymizedStudyId, PatientId = args.PatientId, DicomFile = dicomFile });
                        }

                        _logger.LogInformation($"Uploading DICOM Serie {serieId} for patient {args.PatientId}");

                        await _dicomWebServiceClient.UploadDicomFileAsync(dicomFiles);

                        await _dataPlatformService.UploadSerieToDataPlatform(uploadedData, uploadDate);

                        var existingSerie = await _dicomSerieRepository.GetAsync(serieId);
                        existingSerie.SetCloudUploadDate(cloudUploadDate);

                        await _dicomSerieRepository.UpdateAsync(existingSerie);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Upload Study To Azure failed for patient {args.PatientId}, studyFilePath {studyFilePath}, errorMessage {ex.Message}");
                    }

                }

                Directory.Delete(studyFilePath, true);
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError("Upload Study To Azure failed for patient {patientId}, studyFilePath {studyFilePath}, errorMessage {errorMessage}", args.PatientId, studyFilePath, ex.Message);
                //}

            }

            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new DicomDataSentToAzureEvent()
            };
        }
    }
}
