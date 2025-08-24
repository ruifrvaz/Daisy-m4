using Daisy.Resources.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Daisy.Abilities.HealthCheck.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private HttpClient _client = null!;

        public HealthCheckService(ApplicationSettings settings)
        {
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetService<IHttpClientFactory>()!.CreateClient("DefaultClient");
        }

        public async Task<bool> CheckHealthAsync(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
