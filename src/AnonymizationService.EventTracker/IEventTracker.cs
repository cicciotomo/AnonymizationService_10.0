namespace AnonymizationService.EventTracker
{
    public interface IEventTracker
    {
        void TrackEvent<T>(string eventName, T eventData);
    }
}