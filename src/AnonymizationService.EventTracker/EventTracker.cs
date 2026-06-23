using Microsoft.ApplicationInsights;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace AnonymizationService.EventTracker
{
    internal class EventTracker : IEventTracker, ITransientDependency
    {
        private readonly TelemetryClient telemetryClient;
        private readonly IJsonSerializer jsonSerializer;

        public EventTracker(TelemetryClient telemetryClient, IJsonSerializer jsonSerializer)
        {
            this.telemetryClient = telemetryClient;
            this.jsonSerializer = jsonSerializer;
        }

        public void TrackEvent<T>(string eventName, T eventData)
        {
            var serializedData = jsonSerializer.Serialize(eventData);
            telemetryClient.TrackEvent(eventName, new Dictionary<string, string>() { { "Data", serializedData } });
        }
    }
}
