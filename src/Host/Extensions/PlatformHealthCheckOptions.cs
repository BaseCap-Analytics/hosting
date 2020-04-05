namespace BaseCap.Hosting.Extensions
{
    /// <summary>
    /// Available options for adding Health Checks
    /// </summary>
    public class PlatformHealthCheckOptions
    {
        /// <summary>
        /// Flag for adding a basic Liveness probe
        /// </summary>
        public bool AddBasicLivenessProbe { get; set; }

        /// <summary>
        /// Flag for adding a basic readiness probe
        /// </summary>
        public bool AddBasicReadinessProbe { get; set; }

        /// <summary>
        /// Flag for adding a custom readiness probe
        /// </summary>
        public bool AddCustomReadinessProbe { get; set; }
    }
}
