using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.SynapsePipeline
{
    public class SynapseSettings
    {
        public const string SettingsName = "Settings:SynapseSettings";
        public string Workspace {  get; set; }
        public string SynapseServiceUrl { get; set; }
        public string ApiVersion { get; set; }
        public string PipelineName { get; set; }
        public string RedcapPipelineName { get; set; }
    }
}
