using Newmark.API.Repositories;
using Newmark.API.Repositories.IRepositories;
using Newmark.API.Services;
using Newmark.API.Services.IServices;

namespace Newmark.API.Extensions
{
    /// <summary>
    /// Service registration extensions for large dataset handling capabilities
    /// </summary>
    public static class LargeDatasetServiceExtensions
    {
        /// <summary>
        /// Registers services optimized for handling large datasets (100MB+ blobs)
        /// This includes memory caching, streaming downloads, and performance monitoring
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddLargeDatasetServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add memory caching for performance optimization
            services.AddMemoryCache(options =>
            {
                // Configure cache limits for large datasets
                options.SizeLimit = configuration.GetValue<int>("Caching:MaxCacheSizeMB", 100) * 1024 * 1024; // Default 100MB cache
                options.CompactionPercentage = 0.25; // Remove 25% of items when cache is full
                options.TrackStatistics = true; // Enable cache hit/miss statistics
            });

            // Register the optimized repository for large datasets
            services.AddScoped<IPropertyRepository, OptimizedBlobPropertyRepository>();
            
            // Register the service layer (business logic remains the same)
            services.AddScoped<IPropertyService, PropertyService>();

            // Add logging configuration for performance monitoring
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                
                // Add specific logging for large dataset operations
                builder.AddFilter("Newmark.API.Repositories.OptimizedBlobPropertyRepository", LogLevel.Information);
                builder.AddFilter("Microsoft.Extensions.Caching.Memory", LogLevel.Warning); // Reduce cache noise
            });

            return services;
        }

        /// <summary>
        /// Configures Kestrel server options for handling large payloads
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection ConfigureKestrelForLargePayloads(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
            {
                // Increase limits for large dataset handling
                var maxRequestSizeMB = configuration.GetValue<int>("Kestrel:MaxRequestSizeMB", 250);
                options.Limits.MaxRequestBodySize = maxRequestSizeMB * 1024 * 1024; // Default 250MB
                
                // Increase timeout for large downloads
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(configuration.GetValue<int>("Kestrel:KeepAliveTimeoutMinutes", 15));
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(configuration.GetValue<int>("Kestrel:RequestTimeoutMinutes", 5));
                
                // Configure connection limits
                options.Limits.MaxConcurrentConnections = configuration.GetValue<int>("Kestrel:MaxConnections", 1000);
                options.Limits.MaxConcurrentUpgradedConnections = configuration.GetValue<int>("Kestrel:MaxUpgradedConnections", 1000);
            });

            return services;
        }

        /// <summary>
        /// Adds health checks specifically for large dataset scenarios
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddLargeDatasetHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<LargeDatasetHealthCheck>("large-dataset")
                .AddCheck("memory", () =>
                {
                    // Check available memory
                    var workingSet = GC.GetTotalMemory(false);
                    var workingSetMB = workingSet / (1024 * 1024);
                    
                    // Alert if using more than 500MB (configurable)
                    return workingSetMB < 500 
                        ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy($"Memory usage: {workingSetMB}MB")
                        : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Degraded($"High memory usage: {workingSetMB}MB");
                });

            return services;
        }
    }

    /// <summary>
    /// Health check specifically for large dataset repository operations
    /// </summary>
    public class LargeDatasetHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
    {
        private readonly IPropertyRepository _repository;
        private readonly ILogger<LargeDatasetHealthCheck> _logger;

        public LargeDatasetHealthCheck(IPropertyRepository repository, ILogger<LargeDatasetHealthCheck> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var isHealthy = await _repository.IsHealthyAsync();
                
                if (isHealthy)
                {
                    return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Large dataset repository is accessible and within size limits");
                }
                else
                {
                    return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Degraded("Large dataset repository is accessible but may exceed size limits");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Large dataset health check failed");
                return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Large dataset repository is not accessible", ex);
            }
        }
    }
}
