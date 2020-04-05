using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace BaseCap.Hosting.HealthChecks
{
    /// <summary>
    /// Health Probe to check if the Cloud Service has completed initialization
    /// </summary>
    public class ReadinessProbe : IHealthCheck
    {
        private volatile bool _ready = false;

        /// <summary>
        /// Flag indicating if the Service has completed initialization or not
        /// </summary>
        public bool Ready
        {
            get
            {
                return _ready;
            }
            set
            {
                _ready = value;
            }
        }

        /// <inheritdoc />
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_ready)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Ready"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Not-Ready"));
            }
        }
    }
}
