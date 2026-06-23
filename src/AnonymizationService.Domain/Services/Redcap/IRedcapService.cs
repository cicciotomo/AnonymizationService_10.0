using AnonymizationService.Redcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.Redcap
{
    public interface IRedcapService
    {
        public Task<RedcapStudyConfiguration> GetStudyConfigurationByIdAsync(Guid id);
        public Task UpdateAsync(RedcapStudyConfiguration studyConfig);
    }
}
