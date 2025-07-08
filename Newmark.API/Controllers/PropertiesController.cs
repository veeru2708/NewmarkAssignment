using Microsoft.AspNetCore.Mvc;
using Newmark.API.Models;
using Newmark.API.Services;
using System.ComponentModel.DataAnnotations;
using Newmark.API.Services.IServices;

namespace Newmark.API.Controllers
{
    /// <summary>
    /// RESTful API controller for managing real estate property data
    /// Follows clean architecture principles with proper separation of concerns
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Tags("Properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly ILogger<PropertiesController> _logger;

        /// <summary>
        /// Initializes a new instance of the PropertiesController
        /// </summary>
        /// <param name="propertyService">Service for property business operations</param>
        /// <param name="logger">Logger instance</param>
        public PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger)
        {
            _propertyService = propertyService ?? throw new ArgumentNullException(nameof(propertyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all properties from Azure Blob Storage
        /// </summary>
        /// <returns>A collection of properties with their spaces and rent roll data</returns>
        /// <response code="200">Returns the property data successfully</response>
        /// <response code="500">If there was an error retrieving the data</response>
        [HttpGet]
        [ProducesResponseType(typeof(PropertiesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PropertiesResponse>> GetProperties()
        {
            try
            {
                _logger.LogInformation("Fetching properties data from Azure Blob Storage");
                
                var propertiesData = await _propertyService.GetAllPropertiesAsync();
                
                if (propertiesData == null)
                {
                    _logger.LogError("Failed to retrieve properties data");
                    var errorResponse = ErrorResponse.InternalServerError("Failed to retrieve properties data", HttpContext?.TraceIdentifier ?? "unknown");
                    return StatusCode(500, errorResponse);
                }

                _logger.LogInformation("Successfully retrieved {Count} properties", propertiesData.Properties?.Count ?? 0);
                return Ok(propertiesData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving properties data");
                var errorResponse = ErrorResponse.InternalServerError($"An error occurred: {ex.Message}", HttpContext?.TraceIdentifier ?? "unknown");
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Gets a specific property by its unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the property</param>
        /// <returns>The property with the specified ID</returns>
        /// <response code="200">Returns the property data successfully</response>
        /// <response code="404">If the property with the specified ID is not found</response>
        /// <response code="500">If there was an error retrieving the data</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Property), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Property>> GetProperty([Required] string id)
        {
            try
            {
                _logger.LogInformation("Fetching property with ID: {PropertyId}", id);
                
                var property = await _propertyService.GetPropertyByIdAsync(id);
                
                if (property == null)
                {
                    _logger.LogWarning("Property with ID {PropertyId} not found", id);
                    var notFoundResponse = ErrorResponse.NotFound($"Property with ID '{id}' not found", HttpContext?.TraceIdentifier ?? "unknown");
                    return NotFound(notFoundResponse);
                }

                _logger.LogInformation("Successfully retrieved property: {PropertyName}", property.Name);
                return Ok(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving property with ID: {PropertyId}", id);
                var errorResponse = ErrorResponse.InternalServerError($"An error occurred: {ex.Message}", HttpContext?.TraceIdentifier ?? "unknown");
                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Gets spaces for a specific property
        /// </summary>
        /// <param name="propertyId">The unique identifier of the property</param>
        /// <returns>A collection of spaces for the specified property</returns>
        /// <response code="200">Returns the spaces data successfully</response>
        /// <response code="404">If the property with the specified ID is not found</response>
        /// <response code="500">If there was an error retrieving the data</response>
        [HttpGet("{propertyId}/spaces")]
        [ProducesResponseType(typeof(IEnumerable<Space>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Space>>> GetPropertySpaces([Required] string propertyId)
        {
            try
            {
                _logger.LogInformation("Fetching spaces for property ID: {PropertyId}", propertyId);
                
                var property = await _propertyService.GetPropertyByIdAsync(propertyId);
                
                if (property == null)
                {
                    var notFoundResponse = ErrorResponse.NotFound($"Property with ID '{propertyId}' not found", HttpContext?.TraceIdentifier ?? "unknown");
                    return NotFound(notFoundResponse);
                }

                _logger.LogInformation("Successfully retrieved {Count} spaces for property: {PropertyName}", 
                    property.Spaces?.Count ?? 0, property.Name);
                
                return Ok(property.Spaces ?? new List<Space>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving spaces for property with ID: {PropertyId}", propertyId);
                var errorResponse = ErrorResponse.InternalServerError($"An error occurred: {ex.Message}", HttpContext?.TraceIdentifier ?? "unknown");
                return StatusCode(500, errorResponse);
            }
        }
    }
}
