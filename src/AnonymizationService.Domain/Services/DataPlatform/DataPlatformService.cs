using AnonymizationService.DicomData;
using AnonymizationService.Services.AzureBlob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.DataPlatform
{
	public class DataPlatformService : ITransientDependency, IDataPlatformService
	{

		private readonly DataPlatformSettings _settings;
		private readonly IAzureBlobService _azureBlobService;
		private readonly ILogger<DataPlatformService> _logger;

		public DataPlatformService(IOptions<DataPlatformSettings> dataPlatformSettingsOption, IAzureBlobService azureBlobService, ILogger<DataPlatformService> logger)
		{
			_settings = dataPlatformSettingsOption.Value;
			_azureBlobService = azureBlobService;
			_logger = logger;
		}

		public async Task UploadSerieToDataPlatform(List<DicomInstanceMetadata> dicomInstanceMetadata, DateTime uploadDate)
		{
			foreach (var dicomInstance in dicomInstanceMetadata)
			{
				_logger.LogDebug("Uploading Dicom Instance to Data Platform for patient {patientId}, studyId {studyId}, serieId {serieId}, instanceId {instanceId}", dicomInstance.PatientId, dicomInstance.StudyUid, dicomInstance.SerieUid, dicomInstance.SopInstanceUid);

				var fileFullPath = $"{uploadDate:yyyyMMdd}/{uploadDate:HHmmss}/{dicomInstance.PatientId}/{dicomInstance.StudyUid}/{dicomInstance.SerieUid}/{dicomInstance.SopInstanceUid}.dcm";

				using var fileStream = new MemoryStream();
				await dicomInstance.DicomFile.SaveAsync(fileStream);
				fileStream.Position = 0;

				await _azureBlobService.UploadBlobToContainerAsync(_settings.DicomBlobStorageContainerUrl, fileStream, fileFullPath);
			}
		}
	}
}
