using AnonymizationService.DicomData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Services.DataPlatform
{
	public interface IDataPlatformService
	{
		Task UploadSerieToDataPlatform(List<DicomInstanceMetadata> dicomInstanceMetadata, DateTime uploadDate);
	}
}