using AnonymizationService.Enums;
using AnonymizationService.Services.HttpOperations;
using AnonymizationService.Services.LaboratoryExamService;
using AnonymizationService.Services.ThrottlingManager;
using DemographicWS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.Galileo
{
    [ExposeServices(typeof(IHospitalMasterPatientDataService), typeof(ILaboratoryExamsService), typeof(IClinicalDataService))]
    public class GalileoService : ILaboratoryExamsService, IHospitalMasterPatientDataService, IClinicalDataService, ITransientDependency
    {
        private const string PEOPLE_SAP_ROOT_OID = "SAP";

        private readonly ILogger<GalileoService> _logger;
        private readonly GalileoSettings _galileoSettings;
        private readonly LaboratoryExamSoapProxyClient _laboratoryExamSoapClient;
        private readonly IHttpOperationsService _httpOperationsService;
        private readonly IThrottlingManager _throttlingManager;

        public GalileoService(ILogger<GalileoService> logger
            , IOptionsSnapshot<GalileoSettings> galileoOptions
            , LaboratoryExamSoapProxyClient laboratoryExamSoapClient
            , IHttpOperationsService httpOperationsService
            , IThrottlingManager throttlingManager)
        {
            _logger = logger;
            _galileoSettings = galileoOptions.Value;
            _laboratoryExamSoapClient = laboratoryExamSoapClient;
            _httpOperationsService = httpOperationsService;
            _throttlingManager = throttlingManager;
        }

        public async Task<List<LabResult>> GetPatientLaboratoryExamResultsAsync(string masterPatientIndex, DateTime startDate)
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetPatientLaboratoryExamResults);
            var examList = new List<LabResult>();

            try
            {
                _logger.LogInformation($"Getting Laboratory Exams for patient {masterPatientIndex}");

                var labResultResponse = await _laboratoryExamSoapClient.GetLaboratoryExamsAsync(masterPatientIndex);

                if (labResultResponse != null)
                {
                    var labResults = labResultResponse.LabResult;

                    examList = labResults?.Where(e => e.DAY >= startDate).ToList() ?? new List<LabResult>();
                }

                return examList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Laboratory Exams for patient {masterPatientIndex}");
                throw;
            }
        }

        public async Task<List<LabResult>> GetPatientLaboratoryExamResultsFromDateToDateAsync(string masterPatientIndex, DateTime startDate, DateTime? endDate)
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetPatientLaboratoryExamResults);
            var examList = new List<LabResult>();

            try
            {
                _logger.LogInformation($"Getting Laboratory Exams for patient {masterPatientIndex}");

                var labResultResponse = await _laboratoryExamSoapClient.GetLaboratoryExamsFromDateToDateAsync(masterPatientIndex, startDate, endDate);

                return labResultResponse?.LabResult.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Laboratory Exams for patient {masterPatientIndex}");
                throw;
            }
        }

        public async Task<bool> ValidateMasterPatientIndexAsync(string masterPatientIndex)
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.ValidateMasterPatientIndex);
            try
            {
                _logger.LogInformation($"Validating MPI {masterPatientIndex}");

                using (DemographicServiceClient client = new())
                {
                    client.Endpoint.Address = new EndpointAddress(_galileoSettings.PeopleWsBaseUrl);
                    var response = await client.findByCodeAndRootAsync(masterPatientIndex, PEOPLE_SAP_ROOT_OID, "");

                    return response?.@return != null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating MPI {masterPatientIndex}");
                throw;
            }
        }

        public class ValidatedPatient
        {
            public string MPI;
            public bool Validated;
            public DateTime? BirthDate;
            public string gender;
        }


        public async Task<ValidatedPatient> ValidateMasterPatientIndexGenderAndBirthDateAsync(string masterPatientIndex)
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.ValidateMasterPatientIndex);
            try
            {
                _logger.LogInformation($"Validating MPI {masterPatientIndex}");

                using (DemographicServiceClient client = new())
                {
                    client.Endpoint.Address = new EndpointAddress(_galileoSettings.PeopleWsBaseUrl);
                    var response = await client.findByCodeAndRootAsync(masterPatientIndex, PEOPLE_SAP_ROOT_OID, "");
                    ValidatedPatient validatedPatient = new ValidatedPatient();
                    validatedPatient.Validated = response?.@return != null;

                    if (validatedPatient.Validated)
                    {
                        validatedPatient.MPI = masterPatientIndex;
                        if (response.@return.FirstOrDefault().currentDemographic is null)
                        {
                            //throw new ArgumentNullException("currentDemographic");
                            validatedPatient.BirthDate = null;
                            validatedPatient.gender = string.Empty;
                        }
                        else
                        {
                            validatedPatient.BirthDate = response.@return.FirstOrDefault().currentDemographic.birthdate;
                            validatedPatient.gender = response.@return.FirstOrDefault().currentDemographic.gender.code.ToString();
                        }

                    }

                    return validatedPatient;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating MPI {masterPatientIndex}");
                throw;

            }
        }


        public async Task<string> GetTaxCodeByMasterPatientIndexAsync(string masterPatientIndex)
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.ValidateMasterPatientIndex);
            try
            {
                _logger.LogInformation($"Validating MPI {masterPatientIndex}");

                using (DemographicServiceClient client = new())
                {
                    client.Endpoint.Address = new EndpointAddress(_galileoSettings.PeopleWsBaseUrl);
                    var response = await client.findByCodeAndRootAsync(masterPatientIndex, PEOPLE_SAP_ROOT_OID, "");

                    return response?.@return[0]?.currentDemographic?.taxcode;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating MPI {masterPatientIndex}");
                throw;
            }
        }

        #region IClinicalDataService

        public async Task<List<PatientDocument>> GetPatientClinicalDataResultsAsync(string masterPatientIndex, DateTime startDate)
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetPatientClinicalDataResults);
            var authToken = await GetAuthTokenAsync();
            var address = $"{_galileoSettings.DocumentsDownloadBaseUrl}{_galileoSettings.DocumentsListDownloadApi}";

            _logger.LogInformation("Get patient Clinical Documents of patient {id} from address {address}", masterPatientIndex, address);

            PatientDocumentsRequest body = new PatientDocumentsRequest()
            {
                callingApplication = _galileoSettings.Application,
                token = authToken,
                id_mpi = masterPatientIndex,
                startDate = startDate.ToString("yyyyMMdd"),
                endDate = DateTime.Now.ToString("yyyyMMdd"),
                userId = _galileoSettings.User
            };

            return await _httpOperationsService.PostAsync<List<PatientDocument>>(address, body);
        }

        public async Task<PatientDocumentFile> GetDocumentAsync(int documentId)
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetDocument);
            var authToken = await GetAuthTokenAsync();
            var address = $"{_galileoSettings.DocumentsDownloadBaseUrl}{_galileoSettings.DocumentDownloadApi}";

            _logger.LogInformation("Get clinical document detail of document {documentId} from address {address}", documentId, address);

            PatientFileRequest body = new PatientFileRequest()
            {
                userId = _galileoSettings.User,
                token = authToken,
                documentId = documentId
            };

            return await _httpOperationsService.PostAsync<PatientDocumentFile>(address, body);
        }
        private async Task<string> GetAuthTokenAsync()
        {
            await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetAuthToken);
            try
            {
                var address = $"{_galileoSettings.AuthTokenBaseUrl}{_galileoSettings.AuthTokenApi}";
                //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;


                return await _httpOperationsService.PostAsync(address, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting authorization token");
                throw;
            }
        }

        #endregion IClinicalDataService
    }
}
