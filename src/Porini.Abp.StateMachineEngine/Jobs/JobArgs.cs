using System;
using System.Text.Json.Serialization;

namespace Porini.Abp.StateMachineEngine.Jobs
{
    public class JobArgs
    {
        [JsonPropertyName("stateMachineId")]
        public Guid StateMachineId { get; set; }
    }
}
