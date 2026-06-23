using System.Threading.Tasks;

namespace AnonymizationService.Services.AzureIdentity
{
    public interface IAzureTokenProvider
    {
        Task<string> GetTokenForScopeAsync(string scope);
    }
}
