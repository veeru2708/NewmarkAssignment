using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Newmark.API.Models
{
    /// <summary>
    /// Represents a real estate property with all its details
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Unique identifier for the property
        /// </summary>
        [JsonPropertyName("PropertyId")]
        [Required(ErrorMessage = "Property ID is required")]
        [StringLength(50, ErrorMessage = "Property ID cannot exceed 50 characters")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the property
        /// </summary>
        [JsonPropertyName("PropertyName")]
        [Required(ErrorMessage = "Property name is required")]
        [StringLength(200, ErrorMessage = "Property name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Physical address of the property (not present in current JSON, keeping for future use)
        /// </summary>
        [JsonPropertyName("address")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// List of property features and amenities
        /// </summary>
        [JsonPropertyName("Features")]
        public List<string> Features { get; set; } = new();

        /// <summary>
        /// List of property highlights and selling points
        /// </summary>
        [JsonPropertyName("Highlights")]
        public List<string> Highlights { get; set; } = new();

        /// <summary>
        /// List of available transportation options near the property
        /// </summary>
        [JsonPropertyName("Transportation")]
        public List<TransportationInfo> Transportation { get; set; } = new();

        /// <summary>
        /// List of rentable spaces within the property
        /// </summary>
        [JsonPropertyName("Spaces")]
        public List<Space> Spaces { get; set; } = new();
    }

    /// <summary>
    /// Represents transportation information for a property
    /// </summary>
    public class TransportationInfo
    {
        /// <summary>
        /// Type of transportation (e.g., "Subway", "Bus", "Train", "Bike Share")
        /// </summary>
        [JsonPropertyName("Type")]
        [Required(ErrorMessage = "Transportation type is required")]
        [StringLength(50, ErrorMessage = "Transportation type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Name of the transportation line (e.g., "Green Line", "Route 15")
        /// </summary>
        [JsonPropertyName("Line")]
        [StringLength(100, ErrorMessage = "Line name cannot exceed 100 characters")]
        public string Line { get; set; } = string.Empty;

        /// <summary>
        /// Station name for bike share or other transportation
        /// </summary>
        [JsonPropertyName("Station")]
        [StringLength(100, ErrorMessage = "Station name cannot exceed 100 characters")]
        public string Station { get; set; } = string.Empty;

        /// <summary>
        /// Distance from the property to the transportation option
        /// </summary>
        [JsonPropertyName("Distance")]
        [Required(ErrorMessage = "Distance is required")]
        [StringLength(50, ErrorMessage = "Distance cannot exceed 50 characters")]
        public string Distance { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a rentable space within a property
    /// </summary>
    public class Space
    {
        /// <summary>
        /// Unique identifier for the space
        /// </summary>
        [JsonPropertyName("SpaceId")]
        [Required(ErrorMessage = "Space ID is required")]
        [StringLength(50, ErrorMessage = "Space ID cannot exceed 50 characters")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Display name of the space
        /// </summary>
        [JsonPropertyName("SpaceName")]
        [Required(ErrorMessage = "Space name is required")]
        [StringLength(200, ErrorMessage = "Space name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Type of space (e.g., Office, Warehouse, Retail) - not in current JSON but keeping for future use
        /// </summary>
        [JsonPropertyName("type")]
        [StringLength(50, ErrorMessage = "Space type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Size of the space in square feet - not in current JSON but keeping for future use
        /// </summary>
        [JsonPropertyName("size")]
        [Range(0, int.MaxValue, ErrorMessage = "Size must be a positive number")]
        public int Size { get; set; }

        /// <summary>
        /// Historical rent roll data for the space
        /// </summary>
        [JsonPropertyName("RentRoll")]
        public List<RentRoll> RentRoll { get; set; } = new();
    }

    /// <summary>
    /// Represents rent information for a specific month
    /// </summary>
    public class RentRoll
    {
        /// <summary>
        /// Month abbreviation (e.g., "Jan", "Feb", "Mar")
        /// </summary>
        [JsonPropertyName("Month")]
        [Required(ErrorMessage = "Month is required")]
        [StringLength(10, ErrorMessage = "Month cannot exceed 10 characters")]
        public string Month { get; set; } = string.Empty;

        /// <summary>
        /// Year of the rent record - not in current JSON but keeping for future use
        /// </summary>
        [JsonPropertyName("year")]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100")]
        public int Year { get; set; } = DateTime.Now.Year;

        /// <summary>
        /// Rent amount for the specified month
        /// </summary>
        [JsonPropertyName("Rent")]
        [Range(0, double.MaxValue, ErrorMessage = "Rent must be a positive amount")]
        public decimal Rent { get; set; }
    }

    /// <summary>
    /// Response model containing a collection of properties
    /// </summary>
    public class PropertiesResponse
    {
        /// <summary>
        /// Collection of properties
        /// </summary>
        [JsonPropertyName("properties")]
        public List<Property> Properties { get; set; } = new();
    }
}
