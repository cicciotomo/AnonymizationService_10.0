using System.Threading.Tasks;

namespace AnonymizationService.Services.UploadIncrementalDataManager
{
    public interface IUploadIncrementalDataManager
    {
        Task UploadIncrementalDataAsync();
        Task Initialize();
    }
}
