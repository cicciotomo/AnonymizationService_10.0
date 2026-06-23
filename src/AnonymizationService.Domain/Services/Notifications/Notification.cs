using AnonymizationService.Enums;

namespace AnonymizationService.Services.Notifications
{
    public class Notification
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public MessageType MessageType { get; set; }

    }
}
