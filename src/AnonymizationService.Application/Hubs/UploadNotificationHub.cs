using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;

namespace AnonymizationService.Hubs
{
    [Authorize]
    public class UploadNotificationHub : AbpHub { }
}
