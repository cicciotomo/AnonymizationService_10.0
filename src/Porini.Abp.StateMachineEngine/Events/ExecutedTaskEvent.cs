using System;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class ExecutedTaskEvent
    {
        protected ExecutedTaskEvent() { }
        public ExecutedTaskEvent(Guid stateMachineId, JobEvent executedEvent)
        {
            StateMachineId = stateMachineId;
            ExecutedEvent = executedEvent;
        }

        public Guid StateMachineId { get; set; }
        public JobEvent ExecutedEvent { get; set; }
    }
}
