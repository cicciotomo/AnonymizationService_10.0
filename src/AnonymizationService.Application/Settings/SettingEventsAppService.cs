using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace AnonymizationService.Settings
{
    public class SettingEventsAppService : AnonymizationServiceAppService
    {
        private readonly IConfigurationRefresher _configurationRefresher;

        public SettingEventsAppService(IConfigurationRefresherProvider configurationRefresherProvider)
        {
            _configurationRefresher = configurationRefresherProvider.Refreshers.FirstOrDefault();
        }

        public async Task<SubscriptionValidationResponse> ReceiveEventAsync(EventGridEvent[] events)
        {
            foreach (var eventGridEvent in events)
            {
                if (eventGridEvent.EventType == SystemEventNames.EventGridSubscriptionValidation)
                {
                    var subscriptionValidationEventData = eventGridEvent.Data.ToObjectFromJson<SubscriptionValidationEventData>();
                    string validationCode = subscriptionValidationEventData.ValidationCode;
                    return new SubscriptionValidationResponse() { ValidationResponse = validationCode };
                }
                else if (eventGridEvent.EventType == SystemEventNames.AppConfigurationKeyValueModified)
                {
                    if (eventGridEvent.TryCreatePushNotification(out PushNotification pushNotification))
                    {
                        _configurationRefresher.ProcessPushNotification(pushNotification);
                        await _configurationRefresher.TryRefreshAsync();
                    }
                }
            }

            return new SubscriptionValidationResponse();
        }
    }
}