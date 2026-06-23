using System.Threading.Tasks;

namespace AnonymizationService.Services.HttpOperations
{
    public interface IHttpOperationsService
    {
        Task<T> PostAsync<T>(string address, object body);
        Task<string> PostAsync(string address, object body);
        Task DeleteAsync(string address);
        Task<T> GetAsync<T>(string requestUri, string token = null);
		Task<T> GetFromXmlAsync<T>(string requestUri, string basicAuthUsername = null, string basicAuthPassword = null);
	}
}

