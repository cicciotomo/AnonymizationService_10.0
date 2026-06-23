using System.Threading.Tasks;

namespace AnonymizationService.LaboratoryExams
{
    public interface ILaboratoryCodeMappingService
    {
        Task InitializeMapAsync();
        (string System, string Code) MapLaboratoryCode(string source, string destinationSystem = "");
    }
}