using System;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class FailedJobEvent
    {
        public Guid StateMachineId { get; set; }
        public Exception Exception { get; set; }
    }
}
