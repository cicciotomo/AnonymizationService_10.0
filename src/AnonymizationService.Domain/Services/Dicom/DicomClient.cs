using Azure;
using FellowOakDicom;
using FellowOakDicom.Network;
using FellowOakDicom.Network.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.Dicom
{
    internal class DicomClient : IDicomClient, ITransientDependency
    {
        private readonly DicomSettings _dicomSettings;
        private readonly ILogger<DicomClient> _logger;

        public DicomClient(IOptionsSnapshot<DicomSettings> dicomSettings, ILogger<DicomClient> logger)
        {
            _dicomSettings = dicomSettings.Value;
            _logger = logger;
        }



        public async Task<List<DicomStudy>> RetrieveStudiesByPatientIdAndModalityAsync(string patientId, DateTime startDate, DateTime? endDate, string[] modalities, string pacsSource, string filterTag)
        {
            _logger.LogInformation("Retrieving studies for patient {patientId} and modality {modality}", patientId, modalities);

            //***********************************************************************************************
            try
            {
                //***********************************************************************************************
                // A seconda della sorgente specificata (AGFA / PACS RICERCA) istanziare il relativo client DICOM
                var parametersForPacsSource = SetParametersForPacsSource(pacsSource);
                var client = DicomClientFactory.Create(parametersForPacsSource.ServerHost,
                                                        parametersForPacsSource.ServerPort,
                                                        parametersForPacsSource.UseTls,
                                                        parametersForPacsSource.Aet,
                                                        parametersForPacsSource.ServerAet);
                //***********************************************************************************************

                client.NegotiateAsyncOps();

                var dicomStudiesResult = new List<DicomStudy>();

                string[] filterValues = filterTag.Split('#');

                foreach (var filterValue in filterValues)
                {
                    _logger.LogInformation("Filtering studies with value {filterValue} in tag {filterTag}", filterValue, DicomTag.StudyDescription);

                    var request = CreateStudyRequestByPatientId(patientId, startDate.ToString("yyyyMMdd-"), string.Join('\\', modalities), filterValue);

                    var dicomStudies = new List<DicomStudy>();
                    request.OnResponseReceived += (_, response) =>
                    {
                        _logger.LogInformation($"Response received from Dicom Server: {response.ToString()}");
                        if (response.Status.State == DicomState.Failure)
                        {
                            _logger.LogError($"Failed to retrieve Dicom studies with error {response.Status.Description}");
                            throw new Exception(response.Status.Description);
                        }

                        var studyId = response.Dataset?.GetSingleValue<string>(DicomTag.StudyInstanceUID);
                        if (studyId != null)
                        {
                            var studyDate = response.Dataset.GetSingleValue<DateTime>(DicomTag.StudyDate);
                            dicomStudies.Add(new DicomStudy() { StudyDate = studyDate, StudyId = studyId });
                        }
                    };
                    await client.AddRequestAsync(request);
                    await client.SendAsync();

                    //filtro gli studi per data fine intervallo
                    if (endDate.HasValue)
                    {
                        dicomStudies=dicomStudies.Where(x => x.StudyDate <= endDate).ToList();
                    }
                    dicomStudiesResult.AddRange(dicomStudies);
                }

                return dicomStudiesResult;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);//("Response received from Dicom Server: {dicomResponse}", response.ToString());
                throw;
            }

        }

        private DicomCFindRequest CreateStudyRequestByPatientId(string patientId, string studyDate, string modalitiesInStudy, string filterTag)
        {
            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Study);

            // always add the encoding
            request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");

            // add the dicom tags with empty values that should be included in the result of the QR Server
            request.Dataset.AddOrUpdate(DicomTag.PatientID, "");
            request.Dataset.AddOrUpdate(DicomTag.ModalitiesInStudy, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyDate, "");
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, "");

            // add the dicom tags that contain the filter criterias
            request.Dataset.AddOrUpdate(DicomTag.PatientID, patientId);
            request.Dataset.AddOrUpdate(DicomTag.ModalitiesInStudy, modalitiesInStudy);
            request.Dataset.AddOrUpdate(DicomTag.StudyDate, studyDate);
            request.Dataset.AddOrUpdate(DicomTag.StudyDescription, $"*{filterTag}*");

            return request;
        }


        public async Task<List<DicomSerie>> RetrieveSeriesByStudyIdAsync(string studyId, string[] modalities, string pacsSource)
        {
            _logger.LogInformation("Retrieving series for study {studyId}", studyId);

            //***********************************************************************************************
            // A seconda della sorgente specificata (AGFA / PACS RICERCA) istanziare il relativo client DICOM
            var parametersForPacsSource = SetParametersForPacsSource(pacsSource);
            var client = DicomClientFactory.Create(parametersForPacsSource.ServerHost,
                                                    parametersForPacsSource.ServerPort,
                                                    parametersForPacsSource.UseTls,
                                                    parametersForPacsSource.Aet,
                                                    parametersForPacsSource.ServerAet);
            //***********************************************************************************************
            //var client = DicomClientFactory.Create(_dicomSettings.ServerHost, _dicomSettings.ServerPort, _dicomSettings.UseTls, _dicomSettings.Aet, _dicomSettings.ServerAet);
            client.NegotiateAsyncOps();

            var dicomSeries = new List<DicomSerie>();

            var request = CreateSeriesRequestByStudyUID(studyId);
            request.OnResponseReceived += (req, response) =>
            {
                _logger.LogInformation($"Response received from Dicom Server: {response.ToString()}");

                if (response.Status.State == DicomState.Failure)
                {
                    _logger.LogError($"Failed to retrieve Dicom series with error {response.Status.Description}");
                    throw new Exception(response.Status.Description);
                }

                var serieId = response.Dataset?.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
                if (serieId != null)
                {
                    var serieModality = response.Dataset?.GetSingleValue<string>(DicomTag.Modality);
                    if (modalities.Contains(serieModality))
                    {
                        dicomSeries.Add(new DicomSerie() { Modality = serieModality, SerieId = serieId, StudyId = studyId });
                    }
                }
            };

            await client.AddRequestAsync(request);
            await client.SendAsync();

            return dicomSeries;
        }

        private DicomCFindRequest CreateSeriesRequestByStudyUID(string studyInstanceUID)
        {
            var request = new DicomCFindRequest(DicomQueryRetrieveLevel.Series);

            request.Dataset.AddOrUpdate(new DicomTag(0x8, 0x5), "ISO_IR 100");

            // add the dicom tags with empty values that should be included in the result
            request.Dataset.AddOrUpdate(DicomTag.SeriesInstanceUID, "");
            request.Dataset.AddOrUpdate(DicomTag.Modality, "");
            request.Dataset.AddOrUpdate(DicomTag.NumberOfSeriesRelatedInstances, "");

            // add the dicom tags that contain the filter criterias
            request.Dataset.AddOrUpdate(DicomTag.StudyInstanceUID, studyInstanceUID);

            return request;
        }

        public async Task<bool> RequestMoveForSerieAsync(string studyId, string serieId, string pacsSource)
        {
            _logger.LogInformation("CMove requested for serie {serieId} of study {studyId}", serieId, studyId);
            //***********************************************************************************************
            // A seconda della sorgente specificata (AGFA / PACS RICERCA) istanziare il relativo client DICOM
            var parametersForPacsSource = SetParametersForPacsSource(pacsSource);
            //_logger.LogInformation($"ParametersForPacsSource '{pacsSource}': \n ServerHost: {parametersForPacsSource.ServerHost}\n ServerPort: {parametersForPacsSource.ServerPort}\n UseTls: {parametersForPacsSource.UseTls}\n Aet: {parametersForPacsSource.Aet}\n ServerAet: {parametersForPacsSource.ServerAet}");
            var client = DicomClientFactory.Create(parametersForPacsSource.ServerHost,
                                                    parametersForPacsSource.ServerPort,
                                                    parametersForPacsSource.UseTls,
                                                    parametersForPacsSource.Aet,
                                                    parametersForPacsSource.ServerAet);
            //***********************************************************************************************
            //var client = DicomClientFactory.Create(_dicomSettings.ServerHost, _dicomSettings.ServerPort, _dicomSettings.UseTls, _dicomSettings.Aet, _dicomSettings.ServerAet);

            var cMoveRequest = new DicomCMoveRequest(_dicomSettings.CMoveDestination, studyId, serieId);
            //var cMoveRequest = new DicomCMoveRequest("SRACEPACS", studyId, serieId);

            bool moveSuccessfully = false;
            List<string> errors = new List<string>();
            cMoveRequest.OnResponseReceived += (requ, response) =>
            {
                if (response.Status.State == DicomState.Success)
                {
                    _logger.LogInformation("CMove succeeded for serie {serieId} of study {studyId} on PACS {pacsSource}", serieId, studyId, pacsSource);
                    moveSuccessfully = true;
                }
                else if (response.Status.State == DicomState.Failure)
                {
                    _logger.LogWarning("CMove failed for serie {serieId} of study {studyId} on PACS {pacsSource}, error: {error}", serieId, studyId, pacsSource, response.Status.Description);
                    errors.Add(response.Status.Description);
                    moveSuccessfully = false;
                }
            };
            await client.AddRequestAsync(cMoveRequest);
            await client.SendAsync();

            if (!moveSuccessfully)
            {
                _logger.LogError(string.Join(", ", errors));
                _logger.LogError("CMove failed for serie {serieId} of study {studyId} on PACS {pacsSource}", serieId, studyId, pacsSource);
            }
            return moveSuccessfully;
        }

        public async Task RequestMoveForStudyAsync(string studyId, string pacsSource)
        {
            _logger.LogInformation("CMove requested for study {studyId}", studyId);

            //***********************************************************************************************
            // A seconda della sorgente specificata (AGFA / PACS RICERCA) istanziare il relativo client DICOM
            var parametersForPacsSource = SetParametersForPacsSource(pacsSource);
            var client = DicomClientFactory.Create(parametersForPacsSource.ServerHost,
                                                    parametersForPacsSource.ServerPort,
                                                    parametersForPacsSource.UseTls,
                                                    parametersForPacsSource.Aet,
                                                    parametersForPacsSource.ServerAet);
            //***********************************************************************************************
            //var client = DicomClientFactory.Create(_dicomSettings.ServerHost, _dicomSettings.ServerPort, _dicomSettings.UseTls, _dicomSettings.Aet, _dicomSettings.ServerAet);

            var cMoveRequest = new DicomCMoveRequest(_dicomSettings.CMoveDestination, studyId);
            bool moveSuccessfully = false;
            List<string> errors = new List<string>();
            cMoveRequest.OnResponseReceived += (requ, response) =>
            {
                if (response.Status.State == DicomState.Success)
                {
                    _logger.LogInformation("CMove succeeded for study {studyId}", studyId);
                    moveSuccessfully = true;
                }
                else if (response.Status.State == DicomState.Failure)
                {
                    _logger.LogWarning("CMove failed for study {studyId}, error: {error}", studyId, response.Status.Description);
                    errors.Add(response.Status.Description);
                    moveSuccessfully = false;
                }
            };
            await client.AddRequestAsync(cMoveRequest);
            await client.SendAsync();

            if (!moveSuccessfully)
            {
                _logger.LogError(string.Join(", ", errors));
                throw new Exception(string.Join(", ", errors));
            }
        }


        private ParametersForPacsSource SetParametersForPacsSource(string pacsSource)
        {
            _logger.LogDebug($"Starting SetParametersForPacsSource for PACS source: {pacsSource}");
            ParametersForPacsSource pps = new ParametersForPacsSource();
            if (pacsSource.ToLower() == "pacsagfa".ToLower())
            {
                pps.ServerHost = _dicomSettings.ServerHost;
                pps.ServerPort = _dicomSettings.ServerPort;
                pps.UseTls = _dicomSettings.UseTls;
                pps.Aet = _dicomSettings.Aet;
                pps.ServerAet = _dicomSettings.ServerAet;
            }
            else //if (pacsSource == "pacsricerca")
            {
                pps.ServerHost = _dicomSettings.ServerHost_Ricerca;
                pps.ServerPort = _dicomSettings.ServerPort_Ricerca;
                pps.UseTls = _dicomSettings.UseTls_Ricerca;
                pps.Aet = _dicomSettings.Aet_Ricerca;
                pps.ServerAet = _dicomSettings.ServerAet_Ricerca;
            }
            _logger.LogDebug($"ParametersForPacsSource '{pacsSource}': \n ServerHost: {pps.ServerHost}\n ServerPort: {pps.ServerPort}\n UseTls: {pps.UseTls}\n Aet: {pps.Aet}\n ServerAet: {pps.ServerAet}");
            return pps;
        }

        private class ParametersForPacsSource
        {
            public string ServerHost { get; set; }
            public int ServerPort { get; set; }
            public bool UseTls { get; set; }
            public string Aet { get; set; }
            public string ServerAet { get; set; }
        }
    }
}
