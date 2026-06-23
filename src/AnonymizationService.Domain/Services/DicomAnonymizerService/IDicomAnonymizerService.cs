using System.Threading.Tasks;

namespace AnonymizationService.Services.DicomAnonymizerService
{
    internal interface IDicomAnonymizerService
    {
        Task AnonymizeDicomImagesInFolderAsync(string inputFolder, string outputFolder, AnonymizationParameters anonymizationParameters);
    }
}
