
using AnonymizationService.EventTracker;
using AnonymizationService.HospitalPatients;
using AnonymizationService.Patients;
using AnonymizationService.Services.Galileo;
using AnonymizationService.Services.PatientLoadManager;
using AnonymizationService.StateMachines.Redcap;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using AnonymizationService.Jobs.UpsertFihrRedcapStudyMetadata;
using AnonymizationService.Services.Redcap;
using System.Text.Json;
using System.Reflection.Metadata;
using Volo.Abp.Localization;
using Microsoft.Extensions.Localization;
using AnonymizationService.Localization;
using AnonymizationService.Services.SynapsePipeline;
using AnonymizationService.IntensiveCare;
using AnonymizationService.Permissions;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace AnonymizationService.Redcap
{
    [Authorize(AnonymizationServicePermissions.PatientManagementPermission)]
    public class RedcapAppService : AnonymizationServiceAppService
    {

        private readonly IRepository<RedcapStudyConfiguration,Guid> _redcapStudyConfigurationRepository;
        private readonly IRepository<TransmissionToUseCase, Guid> _trasmissioniVsUseCaseRepository;
        private readonly IRedcapClient _redcapClient;
        public readonly ILogger<RedcapAppService> _logger;
        private readonly IStringLocalizer<AnonymizationServiceResource> _localizer;
        private readonly IServiceProvider _serviceProvider;

        public RedcapAppService(IRepository<RedcapStudyConfiguration, Guid> redcapStudyConfigRepo,
                                IRepository<TransmissionToUseCase, Guid> trasmissioniVsUseCaseRepository,
                             IStringLocalizer<AnonymizationServiceResource> localizer,
                             IRedcapClient redcapClient,
                             ILogger<RedcapAppService> logger,
                             IServiceProvider serviceProvider)
        {
            _redcapStudyConfigurationRepository = redcapStudyConfigRepo;
            _trasmissioniVsUseCaseRepository = trasmissioniVsUseCaseRepository;
            _localizer = localizer;
            _redcapClient = redcapClient;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<PagedResultDto<RedcapStudyDto>> GetListAsync()
        {
            try
            {
                var redcapStudiesConfig = await _redcapStudyConfigurationRepository.GetListAsync();

                var result = new PagedResultDto<RedcapStudyDto>(
                    redcapStudiesConfig.Count,
                    ObjectMapper.Map<List<RedcapStudyConfiguration>, List<RedcapStudyDto>>(redcapStudiesConfig)
                );

                return result;
            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("Error retrieving Redcap studies list.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while retrieving Redcap studies.");
                throw new UserFriendlyException("An error occurred while retrieving the Redcap studies. Please try again later.");
            }
        }

        public async Task<RedcapStudyDetailDto> GetStudyDetailAsync(Guid studyId)
        {
            try
            {
                var redcapStudyConfig = await _redcapStudyConfigurationRepository.GetAsync(studyId);

                var dto = ObjectMapper.Map<RedcapStudyConfiguration,RedcapStudyDetailDto>(redcapStudyConfig);

                return dto;
            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("Error retrieving Redcap studies list.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while retrieving Redcap studies.");
                throw new UserFriendlyException("An error occurred while retrieving the Redcap studies. Please try again later.");
            }
        }

        public async Task<RedcapMetadataDto> GetStudyMetadataAsync(Guid studyId)
        {
            try
            {
                var redcapStudyConfig = await _redcapStudyConfigurationRepository.GetAsync(studyId);

                string metadata_json = await _redcapClient.GetStudyMetadataAsync(redcapStudyConfig.RedcapToken, redcapStudyConfig.Endpoint);

                return new RedcapMetadataDto(GetMetadataFields(metadata_json));
            }
            catch
            {
                _logger.LogWarning("Error retrieving Redcap metadata.");
                throw new UserFriendlyException(code: "Redcap:EndpointNotReachable",message:  _localizer["RedcapEndpointNotReachable"]);
                
            }
        }

        public async Task CreateAsync(CreateRedcapStudyDto createRedcapStudyDto)
        {
            try
            {
                var createRedcapStudy = ObjectMapper.Map<CreateRedcapStudyDto, RedcapStudyConfiguration>(createRedcapStudyDto);

                #region set all fields required as standard configuration
                string metadata_json = await _redcapClient.GetStudyMetadataAsync(createRedcapStudy.RedcapToken, createRedcapStudy.Endpoint);
                var fields = GetMetadataFields(metadata_json);
                createRedcapStudy.SetFields(fields);
                #endregion

                await _redcapStudyConfigurationRepository.InsertAsync(createRedcapStudy);
            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("Error creating Redcap study.");
                throw new UserFriendlyException(_localizer["InvalidRedcapStudyInputMessage"]);
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while creating Redcap study.");
                throw new UserFriendlyException("An error occurred while creating the Redcap study. Please try again later.");
            }
        }

        public async Task UpdateAsync(UpdateRedcapStudyDto updateRedcapStudyDto)
        {
            try
            {
                var updateRedcapStudy = ObjectMapper.Map<UpdateRedcapStudyDto, RedcapStudyConfiguration>(updateRedcapStudyDto);

                await _redcapStudyConfigurationRepository.UpdateAsync(updateRedcapStudy);

            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("Error updating Redcap study.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while updating Redcap study.");
                throw new UserFriendlyException("An error occurred while updating the Redcap study. Please try again later.");
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var study = await _redcapStudyConfigurationRepository.FindAsync(id) ?? throw new EntityNotFoundException();
                await _redcapStudyConfigurationRepository.DeleteAsync(id);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogError("Redcap study not found.");
                throw new UserFriendlyException("The study you are trying to delete does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete redcap study");
                throw new UserFriendlyException("An error occurred while deleting the study. Please try again.");
            }
        }

        public async Task RequestSendingMetadataToFihrAsync(Guid studyConfigurationId)
        {
            try
            {
                _logger.LogInformation(String.Format("Resetting metadata fihr id and trasmission date for study {0} ", studyConfigurationId));
                var study = await _redcapStudyConfigurationRepository.GetAsync(studyConfigurationId) ?? throw new Exception("No Studies found.") ;
                study.ResetMetadataFihrId();
                study.ResetTrasmissionDate();
                await _redcapStudyConfigurationRepository.UpdateAsync(study);

            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("Error sending redcap metadata to fihr.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while sending redcap metadata to fihr.");
                throw new UserFriendlyException("An error occurred while sending redcap metadata to fihr. Please try again later.");
            }
        }

        internal static string GetMetadataFields(string data)
        {
            using (JsonDocument document = JsonDocument.Parse(data))
            {
                var fields = new List<string>();

                // Naviga nell'array JSON
                foreach (var element in document.RootElement.EnumerateArray())
                {
                    // Estrai il valore del campo "id"
                    fields.Add(element.GetProperty("field_name").GetString());
                }

                // Stampa i risultati
                return fields.JoinAsString(",");
            }
        }

        internal async Task<string> GetStudyMetadataJsonAsync(Guid studyId)
        {
            try
            {
                var redcapStudyConfig = await _redcapStudyConfigurationRepository.GetAsync(studyId);

                string metadata_json = await _redcapClient.GetStudyMetadataAsync(redcapStudyConfig.RedcapToken, redcapStudyConfig.Endpoint) ?? throw new UserFriendlyException(_localizer["RedcapEndpointNotReachable"]);


                return metadata_json;
            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("Error retrieving Redcap metadata.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while retrieving Redcap metadata.");
                throw new UserFriendlyException("An error occurred while retrieving Redcap metadata. Please try again later.");
            }
        }

        public async Task SendDataToDataLakeAsync(Guid studyConfigurationId, string filterName, string filterValue, Guid useCaseId)
        {
            var synapsePipelinesService = _serviceProvider.GetService<ISynapsePipelinesService>();
            try
            {
                //RECUPERO DATI FILTRATI DA REDCAP
                _logger.LogInformation(String.Format("Retrieving filtered data from redcap study with id {0} ", studyConfigurationId));
                var study = await _redcapStudyConfigurationRepository.GetAsync(studyConfigurationId) ?? throw new Exception("No Studies found.");
                var data = await _redcapClient.GetStudyDataAsync(study.RedcapToken, study.Endpoint, study.Fields, BuildRequestFilter(filterName, filterValue));
                if (data == null || data == string.Empty || data == "[]") { throw new InvalidOperationException("No study data retrieved for the given parameters"); }
                //SALVA DATI IN TRASMISSIONIVSUSECASE
                var newItemId = await _trasmissioniVsUseCaseRepository.InsertAsync(new TransmissionToUseCase(Guid.NewGuid(), studyConfigurationId, data, useCaseId), autoSave: true);
                //LANCIA PIPELINE RUN CON ID TRASMISSIONIVSUSECASE
                RedcapPipelineParameterDto plParameters = new RedcapPipelineParameterDto();
                plParameters.TransmissionId = newItemId.Id;
                var response = await synapsePipelinesService.CreateRedcapPipelineRunAsync(plParameters);
                CreatePipelineRunAsyncResponse resp = System.Text.Json.JsonSerializer.Deserialize<CreatePipelineRunAsyncResponse>(response);
                //SALVA PIPELINE RUN DATI IN TRASMISSIONIVSUSECASE
                if (resp != null)
                {
                    newItemId.UpdatePipelineRunID(Guid.Parse(resp.runId));
                    newItemId.UpdatePipelineRunCreationTime(DateTime.Now);
                }
                await _trasmissioniVsUseCaseRepository.UpdateAsync(newItemId, autoSave: true);                
            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("Error sending redcap data to datalake.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("An unexpected error occurred while sending redcap data to datalake.");
                throw new UserFriendlyException("An error occurred while sending redcap data to datalake. Please try again later.");
            }
        }

        internal static string BuildRequestFilter(string filterName, string filterValue)
        {
            if((filterName == "") && (filterValue == ""))
            {
                return "";
            } 
            else
            {
                return $"[{filterName}] = '{filterValue}'";
            }
        }

        public class CreatePipelineRunAsyncResponse
        {
            public string runId { get; set; }
        }

    }
}
