using Serilog;
using System;

namespace AnonymizationService.Services.FileSystemDocumentLogService
{
    public interface IFileSystemDocumentLogService
    {
        ILogger CreaLoggerPerDocumento(string path, int documentId);
    }
}
