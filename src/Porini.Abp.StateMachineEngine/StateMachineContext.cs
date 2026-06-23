using System;
using System.Text.Json;

namespace Porini.Abp.StateMachineEngine
{
    public abstract class ContextStateMachine<T> : StateMachine where T : new()
    {
        public ContextStateMachine()
        {
            ContextData = new T();
        }

        public ContextStateMachine(Guid id) : base(id)
        {
            ContextData = new T();
        }

        public T ContextData { get; set; }

        internal override string GetSerializedContext()
            => JsonSerializer.Serialize(ContextData);

        internal override void TrySetSerializedContext(string serializedContextData)
        {
            if (serializedContextData is null)
            {
                return;
            }

            ContextData = JsonSerializer.Deserialize<T>(serializedContextData);
        }
    }
}
