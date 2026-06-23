using FellowOakDicom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Services.DicomWeb
{
    public interface IDicomWebServiceClient
    {
        Task UploadDicomFileAsync(List<DicomFile> dicomFiles);
    }
}
