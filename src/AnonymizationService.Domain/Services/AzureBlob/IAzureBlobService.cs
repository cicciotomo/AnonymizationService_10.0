using System.IO;
using System.Threading.Tasks;

namespace AnonymizationService.Services.AzureBlob
{
	public interface IAzureBlobService
	{
		Task UploadBlobToContainerAsync(string blobContainerUri, Stream stream, string fileFullPath);
	}
}