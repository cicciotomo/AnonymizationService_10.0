using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.UpsertFihrRedcapStudyMetadata
{

    public class UpsertFihrRedcapStudyMetadataArgs : JobArgs
    {
        public string Metadata { get; set; }
        public Guid RedcapStudyConfigurationId { get; set; }
    }
}
