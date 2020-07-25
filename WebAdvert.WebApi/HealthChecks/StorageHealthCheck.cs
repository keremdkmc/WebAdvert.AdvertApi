
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;
using WebAdvert.WebApi.Services;

namespace WebAdvert.WebApi.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IAdvertStorageService _advertStorageService;
        public StorageHealthCheck(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isStorageOk = await _advertStorageService.CheckHealthAsync();
            if (isStorageOk)
                return new HealthCheckResult(HealthStatus.Healthy);
            else
                return new HealthCheckResult(HealthStatus.Unhealthy);
        }
    }
}
