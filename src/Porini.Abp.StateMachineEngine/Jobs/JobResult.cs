using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Text.Json.Serialization;

namespace Porini.Abp.StateMachineEngine.Jobs
{
    public class JobResult
    {
        [JsonPropertyName("stateMachineId")]
        public Guid StateMachineId { get; set; }

        public JobEvent JobResultEvent { get; set; }
    }

}
