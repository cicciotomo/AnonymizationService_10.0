using System.Text.Json;

namespace Porini.Abp.StateMachineEngine.States
{
    public abstract class StatefulState<T> : State, IStatefulState<T> where T : class, new()
    {
        public StatefulState()
        {
            StateData = new T();
        }

        protected T StateData { get; set; }

        public string GetSerializedStateData()
            => JsonSerializer.Serialize(StateData);

        public void TrySetStateData(string dataState)
        {
            if (dataState is null)
            {
                return;
            }

            StateData = JsonSerializer.Deserialize<T>(dataState);
        }
    }
}
