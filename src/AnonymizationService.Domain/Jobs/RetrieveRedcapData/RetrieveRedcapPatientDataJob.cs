using AnonymizationService.Services.DbUri;
using AnonymizationService.Services.Galileo;
using AnonymizationService.StateMachines.Redcap;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using System.Runtime.CompilerServices;
using AnonymizationService.Services.Redcap;
using AnonymizationService.Redcap;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static AnonymizationService.Services.Redcap.RedcapClient;

namespace AnonymizationService.Jobs.RetrieveRedcapData
{
    internal class RetrieveRedcapPatientDataJob : Job<RetrieveRedcapPatientDataJobArgs, JobResult>
    {
        private readonly ILogger<RetrieveRedcapPatientDataJob> _logger;
        private readonly IRedcapClient _redcapClient;
        private readonly IRedcapService _redcapService;

        public RetrieveRedcapPatientDataJob(
                ILogger<RetrieveRedcapPatientDataJob> logger
                , IRedcapClient redcapClient, IRedcapService redcapService
            )
        {
            _logger = logger;
            _redcapClient = redcapClient;
            _redcapService = redcapService;
        }

        public override async Task<JobResult> ExecuteJobAsync(RetrieveRedcapPatientDataJobArgs args)
        {
            _logger.LogInformation($"[{args.StateMachineId}] Retrieving data from RedCap study {args.RedCapStudyId} for patient {args.MasterPatientIndex}");

            //Recupera configurazione studio redcap da DB
            var study = await this._redcapService.GetStudyConfigurationByIdAsync(args.RedCapStudyId);
            _logger.LogInformation($"[{args.StateMachineId}] Redcap study name {study.Name}");

            RedcapReturnData data;
            if (study.Modality == "mpi") {
                _logger.LogInformation($"[{args.StateMachineId}] Retrieving data by MPI");
                data = await this._redcapClient.GetPatientDataByMPIAsync(
                        args.MasterPatientIndex
                        , study.MpiColumnName
                        , study.RedcapToken
                        , study.Endpoint
                        , study.Fields
                        , args.CloudPatientId.ToString());
            } else if (study.Modality == "recordId") {
                _logger.LogInformation($"[{args.StateMachineId}] Retrieving data by RecordID");
                data = await this._redcapClient.GetPatientDataByRecordIdAsync(
                    study.RedcapToken
                    , study.MpiColumnName
                    , study.Endpoint
                    , study.Fields
                    , args.CloudPatientId.ToString()
                    , args.RedCapRecordId);
            }
            else {
                _logger.LogError($"[{args.StateMachineId}] Redcap modality {study.Modality} not allowed");
                throw new Exception($"[{args.StateMachineId}] Redcap modality {study.Modality} not allowed");
            }
            _logger.LogInformation($"[{args.StateMachineId}] Data retrieved");


            var metadata = await _redcapClient.GetStudyMetadataAsync(study.RedcapToken, study.Endpoint);

            var item = new RedcapPatientDataSummary();
            item.RedcapStudy = study.Id;
            item.RedcapRecordId = data.RedCapID;
            item.JsonResponseString = data.Data;
            item.MetadataFhirId = study.MetadataFihrId.ToString();
            item.Metadata = metadata;
            item.Study_name = study.Name;
            item.Token = study.RedcapToken;
            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new RedcapPatientDataRetrievedEvent(item)
            };
        }
    }
}

