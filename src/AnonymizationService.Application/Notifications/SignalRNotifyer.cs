using AnonymizationService.Hubs;
using AnonymizationService.Services.Notifications;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Notifications
{
    internal class SignalRNotifyer : INotifyer, ITransientDependency
    {
        private readonly IHubContext<UploadNotificationHub> _uploadNotificationHubContext;

        public SignalRNotifyer(IHubContext<UploadNotificationHub> uploadNotificationHubContext)
        {
            _uploadNotificationHubContext = uploadNotificationHubContext;
        }

        public async Task SendNotificationToAllAsync(Notification notification)
        {
            await _uploadNotificationHubContext
                .Clients
                .All
                .SendCoreAsync("ReceiveNotification", new object[] { notification });
        }

        public async Task SendNotificationToUserAsync(string userId, Notification notification)
        {
            await _uploadNotificationHubContext
                .Clients
                .User(userId)
                .SendCoreAsync("ReceiveNotification", new object[] { notification });
        }
    }
}
