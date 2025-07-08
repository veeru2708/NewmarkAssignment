using Newmark.API.Models;

namespace Newmark.API.Services.IServices
{
    /// <summary>
    /// Interface for property business logic and operations
    /// Provides business-level operations for property management
    /// </summary>
    public interface IPropertyService
    {
        /// <summary>
        /// Retrieves all properties with business logic applied
        /// </summary>
        /// <returns>Properties response containing all property data, or null if retrieval failed</returns>
        Task<PropertiesResponse?> GetAllPropertiesAsync();

        /// <summary>
        /// Retrieves a specific property by its identifier
        /// </summary>
        /// <param name="propertyId">The unique identifier of the property</param>
        /// <returns>The property if found, null otherwise</returns>
        Task<Property?> GetPropertyByIdAsync(string propertyId);

        /// <summary>
        /// Checks if the property data source is healthy and accessible
        /// </summary>
        /// <returns>True if the service is healthy, false otherwise</returns>
        Task<bool> IsServiceHealthyAsync();
    }
}
