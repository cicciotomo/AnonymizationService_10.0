using System;

namespace Porini.Abp.StateMachineEngine.Events
{
    public class FailedStateMachineEvent
    {
        public Guid StateMachineId { get; set; }
        public Guid? ParentStateMachineId { get; set; }
        public string ErrorMessage { get; set; }
    }
}