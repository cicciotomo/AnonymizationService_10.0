using DemographicWS;
using Hl7.Fhir.ElementModel.Types;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.HttpOperations
{
	public class HttpOperationsService : IHttpOperationsService, ITransientDependency
	{
		public async Task<T> PostAsync<T>(string address, object body)
		{

			using (var httpClientHandler = new HttpClientHandler())
			{
				httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

				using (var httpClient = new HttpClient(httpClientHandler))
				{
					httpClient.Timeout = TimeSpan.FromMinutes(5);
					HttpResponseMessage response = await httpClient.PostAsJsonAsync(address, body);

					response.EnsureSuccessStatusCode();

					if (response.IsSuccessStatusCode && response.Content != null)
					{
						return await response.Content.ReadFromJsonAsync<T>();
					}
				}
			}
			return default;
		}

		public async Task<string> PostAsync(string address, object body)
		{
			using (var httpClientHandler = new HttpClientHandler())
			{
				httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

				using (var httpClient = new HttpClient(httpClientHandler))
				{
                    httpClient.Timeout = TimeSpan.FromMinutes(5);
                    HttpResponseMessage response = await httpClient.PostAsJsonAsync(address, body);

					response.EnsureSuccessStatusCode();

					if (response.IsSuccessStatusCode && response.Content != null)
					{
						return await response.Content.ReadAsStringAsync();
					}
				}
			}
			return null;
		}

		public async Task DeleteAsync(string address)
		{
			using var httpClientHandler = new HttpClientHandler();
			httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
			using var httpClient = new HttpClient(httpClientHandler);
			var response = await httpClient.DeleteAsync(address);
			response.EnsureSuccessStatusCode();
		}

		public async Task<T> GetAsync<T>(string requestUri, string bearerToken = null)
		{
			var httpClientHandler = new HttpClientHandler();

			httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

			var httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            if (bearerToken != null) 
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
			}			
			
			var httpResponse = await httpClient.GetAsync(requestUri);

			if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return default;
			}

			httpResponse.EnsureSuccessStatusCode();

			if (httpResponse.IsSuccessStatusCode && httpResponse.Content != null)
			{				
				var getResultContent = await httpResponse.Content.ReadAsStringAsync();
				return JsonSerializer.Deserialize<T>(getResultContent);
			}

			return default;
		}

		public async Task<T> GetFromXmlAsync<T>(string requestUri, string basicAuthUsername = null, string basicAuthPassword = null)
		{
			var httpClientHandler = new HttpClientHandler();

			httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

			var httpClient = new HttpClient(httpClientHandler);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            if (basicAuthUsername != null && basicAuthPassword != null)
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetBasicAuthHeaderFromUsernameAndPassword(basicAuthUsername, basicAuthPassword));
			}

			var httpResponse = await httpClient.GetAsync(requestUri);

			httpResponse.EnsureSuccessStatusCode();

			if (httpResponse.IsSuccessStatusCode && httpResponse.Content != null)
			{
				var getResultContent = await httpResponse.Content.ReadAsStreamAsync();

				var xmlSerializer = new XmlSerializer(typeof(T));
				return (T)xmlSerializer.Deserialize(getResultContent);
			}

			return default;

		}

		private string GetBasicAuthHeaderFromUsernameAndPassword(string username, string password)
		{
			var authenticationString = $"{username}:{password}";
			return Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
		}
	}
}
