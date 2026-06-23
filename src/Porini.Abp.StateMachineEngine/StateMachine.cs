using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Porini.Abp.StateMachineEngine
{
    public abstract class StateMachine : AuditedEntity<Guid>
    {
        protected StateMachine()
        {
            Id = Guid.NewGuid();
            Completed = false;
        }

        protected StateMachine(Guid id) : base(id)
        {
            Completed = false;
        }

        public abstract StateMachineEvent StateMachineCompletedEventType { get; }
        public State CurrentState { get; protected set; }
        public string CurrentStateName { get; private set; }
        public Guid? ParentStateMachineId { get; protected set; }
        public bool Completed { get; protected set; }
        public bool Failed { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public string LatestStateData { get; protected set; }
        public string LatestStateMachineContext { get; protected set; }
        public bool Started { get; protected set; }

        public abstract State Initialize();

        public State HandleEvent(Event @event)
        {
            if (@event is not RetryStateEvent) return ReactToEvent(@event);

            CurrentState.AlreadyRun = false;
            return CurrentState;
        }

        protected abstract State ReactToEvent(Event receivedEvent);

        protected void SetCompleted()
        {
            Completed = true;
            CurrentState = new StateMachineCompletedState();
        }

        public void SetStarted()
        {
            Started = true;
        }

        public void RebuildCurrentState()
        {
            var stateInstance = Activator.CreateInstance(Type.GetType(CurrentStateName));
            CurrentState = (State)stateInstance;

            if (CurrentState is IStatefulState<object> statefulState)
            {
                statefulState.TrySetStateData(LatestStateData);
            }

            TrySetSerializedContext(LatestStateMachineContext);
        }

        internal virtual string GetSerializedContext()
        {
            return string.Empty;
        }

        internal virtual void TrySetSerializedContext(string contextData) { }

        protected void SetCurrentStateValues()
        {
            CurrentStateName = CurrentState?.GetType().AssemblyQualifiedName;

            if (CurrentState is IStatefulState<object> statefulState)
            {
                LatestStateData = statefulState.GetSerializedStateData();
            }
            else
            {
                LatestStateData = null;
            }

            LatestStateMachineContext = GetSerializedContext();
        }

        public State HandleException(Exception exception)
        {
            CurrentState.HandleException(exception);

            if (CurrentState.ShouldReplay())
            {
                return CurrentState;
            }

            Failed = true;
            ErrorMessage = exception.Message;
            CurrentState = new StateMachineBrokenState();
            SetCurrentStateValues();
            return CurrentState;
        }
    }
}
