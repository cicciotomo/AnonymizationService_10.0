using System.Threading.Tasks;

namespace AnonymizationService.Services.Notifications
{
    public interface INotifyer
    {
        Task SendNotificationToUserAsync(string userId, Notification notification);
        Task SendNotificationToAllAsync(Notification notification);
    }
}
