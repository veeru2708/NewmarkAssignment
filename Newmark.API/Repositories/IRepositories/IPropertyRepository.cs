using Newmark.API.Models;

namespace Newmark.API.Repositories.IRepositories
{
    /// <summary>
    /// Repository interface for property data access operations
    /// Provides a clean abstraction layer over data access logic
    /// </summary>
    public interface IPropertyRepository
    {
        /// <summary>
        /// Retrieves all properties from the data source asynchronously
        /// </summary>
        /// <returns>A collection of properties, or null if retrieval fails</returns>
        Task<IEnumerable<Property>?> GetAllPropertiesAsync();

        /// <summary>
        /// Retrieves a specific property by its identifier asynchronously
        /// </summary>
        /// <param name="propertyId">The unique identifier of the property</param>
        /// <returns>The property if found, null otherwise</returns>
        Task<Property?> GetPropertyByIdAsync(string propertyId);

        /// <summary>
        /// Checks if the data source is available and accessible
        /// </summary>
        /// <returns>True if the data source is healthy, false otherwise</returns>
        Task<bool> IsHealthyAsync();
    }
}
