using System;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class CompletedStateMachineEvent
    {
        public Guid StateMachineId { get; set; }
        public Guid? ParentStateMachineId { get; set; }
        public StateMachineEvent ExecutedEvent { get; set; }
    }
}
