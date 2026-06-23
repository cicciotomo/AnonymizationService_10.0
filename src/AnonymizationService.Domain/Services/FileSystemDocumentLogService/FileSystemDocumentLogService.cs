using Hl7.Fhir.Model;
using Serilog;
using System;

namespace AnonymizationService.Services.FileSystemDocumentLogService
{
    public class FileSystemDocumentLogService : IFileSystemDocumentLogService
    {
        public ILogger CreaLoggerPerDocumento(string path, int documentId) => new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(
                    path: $"{path}{DateTime.Now.ToString("yyyyMMdd")}/document_{documentId}.txt",
                    //path: @"/dicom-images/logs/document_{documentId}.txt",
                    shared: true,
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
                )
                .CreateLogger();
    }
}
