using AnonymizationService.Redcap;
using Microsoft.Extensions.Logging;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

using Volo.Abp;

namespace AnonymizationService.Services.Redcap
{
    public class RedcapService : IRedcapService, ITransientDependency
    {
        private readonly IRepository<RedcapStudyConfiguration, Guid> _redcapStudyConfigurationRepository;
        public readonly ILogger<RedcapService> _logger;

        public RedcapService(IRepository<RedcapStudyConfiguration, Guid> redcapStudyConfigRepo,
                             ILogger<RedcapService> logger)
        {
            _redcapStudyConfigurationRepository = redcapStudyConfigRepo;
            _logger = logger;

        }
        //study -> Data
        public async  Task<RedcapStudyConfiguration> GetStudyConfigurationByIdAsync(Guid id)
        {
            #region Retrieve Redcap Study Configuration
            try
            {
                _logger.LogInformation("Retrieving redcap study...");
                var redcapStudy = await _redcapStudyConfigurationRepository.FindAsync(id);
                return redcapStudy;
            } catch(Exception ex) {
                _logger.LogError("Error in retrieving redcap study...");
                throw new Exception(ex.Message, ex);
            }
            #endregion


        }


        public async System.Threading.Tasks.Task UpdateAsync(RedcapStudyConfiguration studyConfig)
        {
            try
            {

                await _redcapStudyConfigurationRepository.UpdateAsync(studyConfig);

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


    }
}

