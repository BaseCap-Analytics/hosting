using BaseCap.Hosting.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseCap.Hosting.HealthChecks
{
    /// <summary>
    /// Health Probe to check if the Cloud Service is alive
    /// </summary>
    public class LivenessProbe : IHealthCheck
    {
        private readonly ILivenessQueryResponder _responder;

        /// <summary>
        /// Creates a new Liveness Probe to check for process liveness
        /// </summary>
        /// <param name="livenessResponder">The Liveness Responder to check for liveness</param>
        public LivenessProbe(ILivenessQueryResponder livenessResponder)
        {
            if (livenessResponder == null)
            {
                throw new ArgumentNullException(nameof(livenessResponder));
            }

            _responder = livenessResponder;
        }

        /// <inheritdoc />
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return _responder.QueryLivenessAsync(context, cancellationToken);
        }
    }
}
