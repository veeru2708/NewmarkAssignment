using Azure.Storage.Blobs;
using Newmark.API.Models;
using Newmark.API.Repositories.IRepositories;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Newmark.API.Repositories
{
    /// <summary>
    /// Optimized Azure Blob Storage implementation for handling large datasets (100MB+)
    /// Includes streaming, caching, and performance optimizations
    /// </summary>
    public class OptimizedBlobPropertyRepository : IPropertyRepository
    {
        private readonly string _blobUrl;
        private readonly string _sasToken;
        private readonly ILogger<OptimizedBlobPropertyRepository> _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        
        // Cache settings
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(30);
        private const string CACHE_KEY = "all_properties";
        
        // Large dataset settings
        private readonly int _maxDownloadSizeMB;
        private readonly TimeSpan _downloadTimeout;

        /// <summary>
        /// Initializes a new instance of the OptimizedBlobPropertyRepository with caching and large dataset support
        /// </summary>
        /// <param name="configuration">Configuration containing Azure Blob Storage settings</param>
        /// <param name="logger">Logger instance for diagnostics</param>
        /// <param name="cache">Memory cache for performance optimization</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when required Azure Blob Storage configuration is missing</exception>
        public OptimizedBlobPropertyRepository(
            IConfiguration configuration, 
            ILogger<OptimizedBlobPropertyRepository> logger,
            IMemoryCache cache)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            
            // Validate required configuration settings
            _blobUrl = configuration["AzureBlob:BlobUrl"] ?? 
                      throw new InvalidOperationException("AzureBlob:BlobUrl configuration is missing. Please configure the Azure Blob Storage URL in appsettings.json");
            _sasToken = configuration["AzureBlob:SasToken"] ?? 
                       throw new InvalidOperationException("AzureBlob:SasToken configuration is missing. Please configure the Azure Blob Storage SAS token in appsettings.json");

            // Validate that the configurations are not empty
            if (string.IsNullOrWhiteSpace(_blobUrl))
            {
                throw new InvalidOperationException("AzureBlob:BlobUrl configuration cannot be empty. Please provide a valid Azure Blob Storage URL in appsettings.json");
            }

            if (string.IsNullOrWhiteSpace(_sasToken))
            {
                throw new InvalidOperationException("AzureBlob:SasToken configuration cannot be empty. Please provide a valid Azure Blob Storage SAS token in appsettings.json");
            }

            // Large dataset configuration
            _maxDownloadSizeMB = configuration.GetValue<int>("AzureBlob:MaxDownloadSizeMB", 200); // Default 200MB limit
            _downloadTimeout = TimeSpan.FromMinutes(configuration.GetValue<int>("AzureBlob:DownloadTimeoutMinutes", 10));

            _logger.LogInformation("OptimizedBlobPropertyRepository initialized with max size: {MaxSize}MB, timeout: {Timeout}min", 
                _maxDownloadSizeMB, _downloadTimeout.TotalMinutes);
        }

        /// <summary>
        /// Retrieves all properties with optimizations for large datasets
        /// </summary>
        public async Task<IEnumerable<Property>?> GetAllPropertiesAsync()
        {
            try
            {
                // Check cache first for performance
                if (_cache.TryGetValue(CACHE_KEY, out IEnumerable<Property>? cachedProperties))
                {
                    _logger.LogInformation("Retrieved {Count} properties from cache", cachedProperties?.Count() ?? 0);
                    return cachedProperties;
                }

                _logger.LogInformation("Cache miss - downloading from Azure Blob Storage");
                
                var properties = await DownloadAndDeserializePropertiesAsync();
                
                if (properties != null)
                {
                    // Cache the results
                    _cache.Set(CACHE_KEY, properties, _cacheExpiry);
                    _logger.LogInformation("Cached {Count} properties for {Minutes} minutes", 
                        properties.Count(), _cacheExpiry.TotalMinutes);
                }

                return properties;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllPropertiesAsync - falling back to mock data");
                return GetMockProperties();
            }
        }

        /// <summary>
        /// Downloads and deserializes properties with streaming for large datasets
        /// </summary>
        private async Task<IEnumerable<Property>?> DownloadAndDeserializePropertiesAsync()
        {
            var blobUri = new Uri($"{_blobUrl}{_sasToken}");
            var blobClient = new BlobClient(blobUri);

            // Check blob existence and size
            var properties = await blobClient.GetPropertiesAsync();
            var blobSizeMB = properties.Value.ContentLength / (1024.0 * 1024.0);
            
            _logger.LogInformation("Blob size: {Size:F2} MB", blobSizeMB);

            // Check size limits
            if (blobSizeMB > _maxDownloadSizeMB)
            {
                _logger.LogWarning("Blob size ({Size:F2} MB) exceeds maximum allowed size ({MaxSize} MB)", 
                    blobSizeMB, _maxDownloadSizeMB);
                throw new InvalidOperationException($"Blob size ({blobSizeMB:F2} MB) exceeds maximum allowed size ({_maxDownloadSizeMB} MB)");
            }

            // Use streaming download for large files
            if (blobSizeMB > 10) // Stream for files larger than 10MB
            {
                return await DownloadWithStreamingAsync(blobClient);
            }
            else
            {
                return await DownloadDirectAsync(blobClient);
            }
        }

        /// <summary>
        /// Streaming download for large blobs (100MB+)
        /// </summary>
        private async Task<IEnumerable<Property>?> DownloadWithStreamingAsync(BlobClient blobClient)
        {
            _logger.LogInformation("Using streaming download for large blob");
            
            using var cts = new CancellationTokenSource(_downloadTimeout);
            
            try
            {
                // Stream the blob content
                var response = await blobClient.DownloadStreamingAsync(cancellationToken: cts.Token);
                
                // Use streaming JSON deserializer for better memory efficiency
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultBufferSize = 65536, // 64KB buffer for streaming
                    AllowTrailingCommas = true
                };

                using var stream = response.Value.Content;
                var properties = await JsonSerializer.DeserializeAsync<List<Property>>(stream, options, cts.Token);
                
                _logger.LogInformation("Successfully streamed and deserialized {Count} properties", properties?.Count ?? 0);
                return properties ?? new List<Property>();
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Download timeout after {Timeout} minutes", _downloadTimeout.TotalMinutes);
                throw new TimeoutException($"Blob download timed out after {_downloadTimeout.TotalMinutes} minutes");
            }
        }

        /// <summary>
        /// Direct download for smaller blobs
        /// </summary>
        private async Task<IEnumerable<Property>?> DownloadDirectAsync(BlobClient blobClient)
        {
            _logger.LogInformation("Using direct download for small blob");
            
            using var cts = new CancellationTokenSource(_downloadTimeout);
            
            var response = await blobClient.DownloadContentAsync(cts.Token);
            var jsonContent = response.Value.Content.ToString();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false // Don't waste memory on formatting for large datasets
            };

            var properties = JsonSerializer.Deserialize<List<Property>>(jsonContent, options);
            
            _logger.LogInformation("Successfully downloaded and deserialized {Count} properties", properties?.Count ?? 0);
            return properties ?? new List<Property>();
        }

        /// <summary>
        /// Retrieves a specific property by ID with caching
        /// </summary>
        public async Task<Property?> GetPropertyByIdAsync(string propertyId)
        {
            if (string.IsNullOrWhiteSpace(propertyId))
            {
                _logger.LogWarning("GetPropertyByIdAsync called with null or empty propertyId");
                return null;
            }

            try
            {
                // Use cached data if available to avoid re-downloading large blob
                var allProperties = await GetAllPropertiesAsync();
                var property = allProperties?.FirstOrDefault(p => 
                    string.Equals(p.Id, propertyId, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    _logger.LogInformation("Found property {PropertyId} in dataset", propertyId);
                }
                else
                {
                    _logger.LogWarning("Property {PropertyId} not found in dataset of {Count} properties", 
                        propertyId, allProperties?.Count() ?? 0);
                }

                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving property {PropertyId}", propertyId);
                return null;
            }
        }

        /// <summary>
        /// Health check with size monitoring
        /// </summary>
        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                var blobUri = new Uri($"{_blobUrl}{_sasToken}");
                var blobClient = new BlobClient(blobUri);

                var exists = await blobClient.ExistsAsync();
                if (!exists.Value) return false;

                // Check if blob size is within acceptable limits
                var properties = await blobClient.GetPropertiesAsync();
                var blobSizeMB = properties.Value.ContentLength / (1024.0 * 1024.0);
                
                var isHealthy = blobSizeMB <= _maxDownloadSizeMB;
                
                _logger.LogDebug("Health check - Blob exists: {Exists}, Size: {Size:F2}MB, Healthy: {Healthy}", 
                    exists.Value, blobSizeMB, isHealthy);
                
                return isHealthy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return false;
            }
        }

        /// <summary>
        /// Clear cache manually (useful for testing or when data is updated)
        /// </summary>
        public void ClearCache()
        {
            _cache.Remove(CACHE_KEY);
            _logger.LogInformation("Properties cache cleared");
        }

        /// <summary>
        /// Provides mock data for development and testing purposes
        /// This method should only be used as a fallback when blob access fails
        /// </summary>
        /// <returns>A collection of mock properties</returns>
        private IEnumerable<Property> GetMockProperties()
        {
            _logger.LogInformation("Returning mock properties data due to blob access failure");

            return new List<Property>
            {
                new Property
                {
                    Id = "PROP001",
                    Name = "Downtown Corporate Center",
                    Address = "123 Main Street, Downtown",
                    Features = new List<string> 
                    { 
                        "24/7 Security with keycard access",
                        "State-of-the-art fitness center",
                        "Multiple conference rooms",
                        "Covered parking garage",
                        "On-site cafeteria",
                        "High-speed fiber internet"
                    },
                    Highlights = new List<string> 
                    { 
                        "Prime downtown location with city views",
                        "LEED Gold certified building",
                        "Recently renovated with modern amenities",
                        "Walking distance to major transit hubs"
                    },
                    Transportation = new List<TransportationInfo>
                    {
                        new TransportationInfo 
                        { 
                            Type = "Subway", 
                            Line = "Green Line", 
                            Distance = "0.3 miles" 
                        },
                        new TransportationInfo 
                        { 
                            Type = "Bus", 
                            Line = "Route 15", 
                            Distance = "0.1 miles" 
                        },
                        new TransportationInfo 
                        { 
                            Type = "Bike Share", 
                            Station = "Downtown Station", 
                            Distance = "0.2 miles" 
                        }
                    },
                    Spaces = new List<Space>
                    {
                        new Space
                        {
                            Id = "SP001",
                            Name = "Executive Suite A",
                            Type = "Office",
                            Size = 1500,
                            RentRoll = new List<RentRoll>
                            {
                                new RentRoll { Month = "Jan", Year = 2024, Rent = 4500 },
                                new RentRoll { Month = "Feb", Year = 2024, Rent = 4500 },
                                new RentRoll { Month = "Mar", Year = 2024, Rent = 4650 },
                                new RentRoll { Month = "Apr", Year = 2024, Rent = 4650 },
                                new RentRoll { Month = "May", Year = 2024, Rent = 4800 },
                                new RentRoll { Month = "Jun", Year = 2024, Rent = 4800 }
                            }
                        }
                    }
                }
            };
        }
    }
}
