using Azure.Storage.Blobs;
using Newmark.API.Models;
using Newmark.API.Repositories.IRepositories;
using System.Text.Json;

namespace Newmark.API.Repositories
{
    /// <summary>
    /// Azure Blob Storage implementation of the property repository
    /// Handles all data access operations for properties stored in Azure Blob Storage
    /// </summary>
    public class BlobPropertyRepository : IPropertyRepository
    {
        private readonly string _blobUrl;
        private readonly string _sasToken;
        private readonly ILogger<BlobPropertyRepository> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the BlobPropertyRepository
        /// </summary>
        /// <param name="configuration">Configuration containing Azure Blob Storage settings</param>
        /// <param name="logger">Logger instance for diagnostics</param>
        /// <exception cref="ArgumentNullException">Thrown when configuration or logger is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when required Azure Blob Storage configuration is missing or empty</exception>
        public BlobPropertyRepository(IConfiguration configuration, ILogger<BlobPropertyRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
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

            _logger.LogInformation("BlobPropertyRepository initialized with blob URL configured");
        }

        /// <summary>
        /// Retrieves all properties from Azure Blob Storage asynchronously
        /// </summary>
        /// <returns>A collection of properties, or null if retrieval fails</returns>
        public async Task<IEnumerable<Property>?> GetAllPropertiesAsync()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all properties from Azure Blob Storage");

                var blobUri = new Uri($"{_blobUrl}{_sasToken}");
                var blobClient = new BlobClient(blobUri);

                // Check if blob exists
                var exists = await blobClient.ExistsAsync();
                if (!exists.Value)
                {
                    _logger.LogWarning("Blob does not exist at the specified URL");
                    return GetMockProperties(); // Fallback to mock data for development
                }

                // Download blob content
                var response = await blobClient.DownloadContentAsync();
                var jsonContent = response.Value.Content.ToString();

                _logger.LogInformation("Successfully downloaded blob content. Size: {Size} characters", jsonContent.Length);

                // Deserialize JSON to C# models
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var properties = JsonSerializer.Deserialize<List<Property>>(jsonContent, options);

                _logger.LogInformation("Successfully deserialized {Count} properties", properties?.Count ?? 0);

                return properties ?? new List<Property>();
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, "Azure Request failed: {StatusCode} - {Message}", 
                    ex.Status, ex.Message);
                
                // Return mock data for development if blob access fails
                return GetMockProperties();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON data: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving properties data");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a specific property by its identifier asynchronously
        /// </summary>
        /// <param name="propertyId">The unique identifier of the property</param>
        /// <returns>The property if found, null otherwise</returns>
        public async Task<Property?> GetPropertyByIdAsync(string propertyId)
        {
            if (string.IsNullOrWhiteSpace(propertyId))
            {
                _logger.LogWarning("GetPropertyByIdAsync called with null or empty propertyId");
                return null;
            }

            try
            {
                _logger.LogInformation("Attempting to retrieve property with ID: {PropertyId}", propertyId);

                var allProperties = await GetAllPropertiesAsync();
                var property = allProperties?.FirstOrDefault(p => 
                    string.Equals(p.Id, propertyId, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    _logger.LogInformation("Successfully found property with ID: {PropertyId}", propertyId);
                }
                else
                {
                    _logger.LogWarning("Property with ID: {PropertyId} not found", propertyId);
                }

                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving property with ID: {PropertyId}", propertyId);
                return null;
            }
        }

        /// <summary>
        /// Checks if the Azure Blob Storage data source is available and accessible
        /// </summary>
        /// <returns>True if the data source is healthy, false otherwise</returns>
        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                _logger.LogDebug("Performing health check on Azure Blob Storage");

                var blobUri = new Uri($"{_blobUrl}{_sasToken}");
                var blobClient = new BlobClient(blobUri);

                var exists = await blobClient.ExistsAsync();
                var isHealthy = exists.Value;

                _logger.LogDebug("Health check result: {IsHealthy}", isHealthy);
                return isHealthy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed for Azure Blob Storage");
                return false;
            }
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
                        },
                        new Space
                        {
                            Id = "SP002",
                            Name = "Open Floor Plan B",
                            Type = "Office",
                            Size = 2200,
                            RentRoll = new List<RentRoll>
                            {
                                new RentRoll { Month = "Jan", Year = 2024, Rent = 6600 },
                                new RentRoll { Month = "Feb", Year = 2024, Rent = 6600 },
                                new RentRoll { Month = "Mar", Year = 2024, Rent = 6800 },
                                new RentRoll { Month = "Apr", Year = 2024, Rent = 6800 },
                                new RentRoll { Month = "May", Year = 2024, Rent = 7000 },
                                new RentRoll { Month = "Jun", Year = 2024, Rent = 7000 }
                            }
                        },
                        new Space
                        {
                            Id = "SP003",
                            Name = "Conference Center C",
                            Type = "Meeting",
                            Size = 800,
                            RentRoll = new List<RentRoll>
                            {
                                new RentRoll { Month = "Jan", Year = 2024, Rent = 2400 },
                                new RentRoll { Month = "Feb", Year = 2024, Rent = 2400 },
                                new RentRoll { Month = "Mar", Year = 2024, Rent = 2500 },
                                new RentRoll { Month = "Apr", Year = 2024, Rent = 2500 },
                                new RentRoll { Month = "May", Year = 2024, Rent = 2600 },
                                new RentRoll { Month = "Jun", Year = 2024, Rent = 2600 }
                            }
                        }
                    }
                },
                new Property
                {
                    Id = "PROP002",
                    Name = "Industrial Business Park",
                    Address = "456 Industrial Blvd, Manufacturing District",
                    Features = new List<string> 
                    { 
                        "Free parking for 200+ vehicles",
                        "Loading docks with hydraulic lifts",
                        "24/7 security patrol",
                        "On-site maintenance team",
                        "Climate-controlled storage",
                        "Truck-friendly access roads"
                    },
                    Highlights = new List<string> 
                    { 
                        "Strategic location near major highways",
                        "Flexible warehouse and office combinations",
                        "Competitive lease rates",
                        "Established business community"
                    },
                    Transportation = new List<TransportationInfo>
                    {
                        new TransportationInfo 
                        { 
                            Type = "Bus", 
                            Line = "Route 22", 
                            Distance = "0.5 miles" 
                        },
                        new TransportationInfo 
                        { 
                            Type = "Highway", 
                            Line = "Interstate 95", 
                            Distance = "1.2 miles" 
                        }
                    },
                    Spaces = new List<Space>
                    {
                        new Space
                        {
                            Id = "SP004",
                            Name = "Warehouse Unit 1",
                            Type = "Warehouse",
                            Size = 5000,
                            RentRoll = new List<RentRoll>
                            {
                                new RentRoll { Month = "Jan", Year = 2024, Rent = 8500 },
                                new RentRoll { Month = "Feb", Year = 2024, Rent = 8500 },
                                new RentRoll { Month = "Mar", Year = 2024, Rent = 8750 },
                                new RentRoll { Month = "Apr", Year = 2024, Rent = 8750 },
                                new RentRoll { Month = "May", Year = 2024, Rent = 9000 },
                                new RentRoll { Month = "Jun", Year = 2024, Rent = 9000 }
                            }
                        },
                        new Space
                        {
                            Id = "SP005",
                            Name = "Office Suite 2",
                            Type = "Office",
                            Size = 1200,
                            RentRoll = new List<RentRoll>
                            {
                                new RentRoll { Month = "Jan", Year = 2024, Rent = 3000 },
                                new RentRoll { Month = "Feb", Year = 2024, Rent = 3000 },
                                new RentRoll { Month = "Mar", Year = 2024, Rent = 3100 },
                                new RentRoll { Month = "Apr", Year = 2024, Rent = 3100 },
                                new RentRoll { Month = "May", Year = 2024, Rent = 3200 },
                                new RentRoll { Month = "Jun", Year = 2024, Rent = 3200 }
                            }
                        }
                    }
                }
            };
        }
    }
}
