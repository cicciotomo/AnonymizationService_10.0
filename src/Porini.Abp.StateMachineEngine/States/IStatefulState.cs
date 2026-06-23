namespace Porini.Abp.StateMachineEngine.States
{
    internal interface IStatefulState<out T>
    {
        public string GetSerializedStateData();

        public void TrySetStateData(string dataState);
    }
}
