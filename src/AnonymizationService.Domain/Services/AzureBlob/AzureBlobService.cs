using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.AzureBlob
{
	public class AzureBlobService : ITransientDependency, IAzureBlobService
    {
        private readonly ILogger<AzureBlobService> _logger;

        public AzureBlobService(ILogger<AzureBlobService> logger)
        {
            _logger = logger;
        }

        public async Task UploadBlobToContainerAsync(string blobContainerUri, Stream stream, string fileFullPath)
        {
            var azureCredentials = new DefaultAzureCredential();
            _logger.LogInformation("Uploading blob to container {containerUrl} with path {fileFullPath}", blobContainerUri, fileFullPath);

			var blobStorageClient = new BlobContainerClient(new Uri(blobContainerUri), azureCredentials);
            stream.Position = 0;
			await blobStorageClient.UploadBlobAsync(fileFullPath, stream);
		}

	}
}
