using AnonymizationService.Services.Dicom;
using AnonymizationService.Settings;
using AnonymizationService.StateMachines.Dicom;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using DicomSerie = AnonymizationService.DicomData.DicomSerie;

namespace AnonymizationService.Jobs.RetrieveDicomData
{
    internal class RetrieveDicomDataJob : Job<RetrieveDicomDataArgs, JobResult>
    {
        private readonly IDicomClient _dicomClient;
        private readonly IOptionsSnapshot<DicomStoreSettings> _dicomStoreSettings;
        private readonly ILogger<RetrieveDicomDataJob> _logger;
        private readonly IRepository<DicomSerie> _dicomSeriesRepository;
        private readonly DicomSettings _dicomSettings;

        public RetrieveDicomDataJob(IDicomClient dicomClient, IOptionsSnapshot<DicomStoreSettings> dicomStoreSettings, ILogger<RetrieveDicomDataJob> logger, IRepository<DicomSerie> dicomSeriesRepository, IOptionsSnapshot<DicomSettings> dicomSettings)
        {
            _dicomClient = dicomClient;
            _dicomStoreSettings = dicomStoreSettings;
            _logger = logger;
            _dicomSeriesRepository = dicomSeriesRepository;
            _dicomSettings = dicomSettings.Value;
        }

        public override async Task<JobResult> ExecuteJobAsync(RetrieveDicomDataArgs args)
        {
            _logger.LogInformation("Retrieving DICOM images for patient {id}", args.CloudPatientId);
            var dicomStudies = await _dicomClient.RetrieveStudiesByPatientIdAndModalityAsync(args.MasterPatientIndex, args.StartDate, args.EndDate, args.Modalities, args.PacsSource, args.DicomParametersJson.StudyDescription);

            _logger.LogInformation("Retrieved {num} DICOM images for patient {id}", dicomStudies.Count, args.CloudPatientId);

            var dicomStudyEntities = new List<DicomData.DicomStudy>();

            foreach (var dicomStudy in dicomStudies)
            {
                var serverAET = string.Empty;
                if (args.PacsSource.ToLower() == "pacsagfa".ToLower())
                {
                    serverAET = _dicomSettings.ServerAet?.Trim();
                }
                else
                {
                    serverAET = _dicomSettings.ServerAet_Ricerca?.Trim();
                }
                serverAET = SanitizeFolderName(string.IsNullOrWhiteSpace(serverAET) ? "UNKNOWN_AE" : serverAET);
                var studyFilePath = Path.Combine(_dicomStoreSettings.Value.DicomImagesBaseDirectory, serverAET, dicomStudy.StudyId);

                var dicomStudyEntity = new DicomData.DicomStudy(dicomStudy.StudyId, args.CloudPatientId,
                    dicomStudy.StudyDate, studyFilePath);

                _logger.LogInformation("Retrieving DICOM detail for study {studyId} for patient {id}",
                    dicomStudy.StudyId, args.CloudPatientId);

                var dicomSeries = await _dicomClient.RetrieveSeriesByStudyIdAsync(dicomStudy.StudyId, args.Modalities, args.PacsSource);
                foreach (var serie in dicomSeries.Where(x => x.Modality.IsIn(args.Modalities)))
                {
                    if (args.DicomParametersJson?.MoveToPacsRicerca == true)
                    {
                        await _dicomClient.RequestMoveForSerieAsync(serie.StudyId, serie.SerieId, args.PacsSource);
                    }
                    else
                    {
                        var dbSerie = await _dicomSeriesRepository.FindAsync(s => s.Id == serie.SerieId);
                        if (dbSerie?.CloudUploadDate != null)
                        {
                            _logger.LogInformation("Dicom Serie {serieId} for study {studyId} already downloaded", serie.SerieId, serie.StudyId);
                            continue;
                        }

                        _logger.LogInformation(
                            "Requesting CMOVE for serie {serieId} of study {studyId} for patient {id} on PACS {pacsSource} in {folder}", serie.SerieId,
                            dicomStudy.StudyId, args.CloudPatientId, args.PacsSource, studyFilePath);

                        var moveSuccessfully = await _dicomClient.RequestMoveForSerieAsync(serie.StudyId, serie.SerieId, args.PacsSource);

                        if (moveSuccessfully)
                        {
                            dicomStudyEntity.AddDicomSerie(new DicomSerie(
                                id: serie.SerieId,
                                dicomStudyId: dicomStudyEntity.Id,
                                modality: serie.Modality
                            ));
                        }
                    }

                }

                dicomStudyEntities.Add(dicomStudyEntity);
            }


            if (args.DicomParametersJson?.MoveToPacsRicerca == true)
            {
                return new JobResult()
                {
                    StateMachineId = args.StateMachineId,
                    JobResultEvent = new DicomDataMovedToPacsRicercaEvent()
                };
            }
            else
            {
                return new JobResult()
                {
                    StateMachineId = args.StateMachineId,
                    JobResultEvent = new DicomDataRetrievedEvent(dicomStudyEntities)
                };
            }


        }

        private static string SanitizeFolderName(string value)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                value = value.Replace(c, '_');
            }

            return value;
        }
    }
}
