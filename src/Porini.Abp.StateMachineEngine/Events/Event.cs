namespace Porini.Abp.StateMachineEngine.Events
{
    public class StateMachineEvent : Event { }
    public class JobEvent : Event { }
    public sealed class RetryStateEvent : Event { }
    public abstract class Event { }
}
