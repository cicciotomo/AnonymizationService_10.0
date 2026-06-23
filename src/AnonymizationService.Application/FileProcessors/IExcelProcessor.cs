using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AnonymizationService.FileProcessors
{
    public interface IExcelProcessor
    {
        Task<IEnumerable<T>> MapFromExcelFileAsync<T>(Stream fileStream);
    }
}