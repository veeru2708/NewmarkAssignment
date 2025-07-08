using Newmark.API.Models;
using Newmark.API.Repositories.IRepositories;
using Newmark.API.Services.IServices;

namespace Newmark.API.Services
{
    /// <summary>
    /// Service implementation for property business logic and operations
    /// Acts as a bridge between controllers and repository layer
    /// Handles business logic, validation, and orchestration of property operations
    /// </summary>
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILogger<PropertyService> _logger;

        /// <summary>
        /// Initializes a new instance of the PropertyService
        /// </summary>
        /// <param name="propertyRepository">Repository for property data access</param>
        /// <param name="logger">Logger instance for diagnostics</param>
        /// <exception cref="ArgumentNullException">Thrown when repository or logger is null</exception>
        public PropertyService(IPropertyRepository propertyRepository, ILogger<PropertyService> logger)
        {
            _propertyRepository = propertyRepository ?? throw new ArgumentNullException(nameof(propertyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all properties with business logic applied
        /// </summary>
        /// <returns>Properties response containing all property data, or null if retrieval failed</returns>
        public async Task<PropertiesResponse?> GetAllPropertiesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all properties through service layer");

                var properties = await _propertyRepository.GetAllPropertiesAsync();
                
                if (properties == null)
                {
                    _logger.LogWarning("Repository returned null for properties data");
                    return null;
                }

                var propertiesList = properties.ToList();
                
                // Apply any business logic here (sorting, filtering, validation, etc.)
                var sortedProperties = propertiesList
                    .OrderBy(p => p.Name)
                    .ToList();

                _logger.LogInformation("Successfully retrieved and processed {Count} properties", 
                    sortedProperties.Count);

                return new PropertiesResponse
                {
                    Properties = sortedProperties
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in PropertyService while retrieving all properties");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a specific property by its identifier
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
                _logger.LogInformation("Retrieving property with ID: {PropertyId} through service layer", propertyId);

                var property = await _propertyRepository.GetPropertyByIdAsync(propertyId);
                
                if (property != null)
                {
                    // Apply any business logic here (validation, enrichment, etc.)
                    _logger.LogInformation("Successfully retrieved property: {PropertyName} (ID: {PropertyId})", 
                        property.Name, property.Id);
                }
                else
                {
                    _logger.LogWarning("Property with ID: {PropertyId} not found", propertyId);
                }

                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in PropertyService while retrieving property with ID: {PropertyId}", propertyId);
                return null;
            }
        }

        /// <summary>
        /// Checks if the property data source is healthy and accessible
        /// </summary>
        /// <returns>True if the service is healthy, false otherwise</returns>
        public async Task<bool> IsServiceHealthyAsync()
        {
            try
            {
                _logger.LogDebug("Performing health check through service layer");

                var isHealthy = await _propertyRepository.IsHealthyAsync();
                
                _logger.LogDebug("Service health check result: {IsHealthy}", isHealthy);
                return isHealthy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during service health check");
                return false;
            }
        }
    }
}
