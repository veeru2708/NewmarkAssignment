using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newmark.API.Controllers;
using Newmark.API.Services.IServices;
using Newmark.API.Models;

namespace Newmark.API.Tests.Controllers
{
    /// <summary>
    /// Unit tests for the PropertiesController with repository pattern
    /// Demonstrates testing patterns and best practices
    /// </summary>
    public class PropertiesControllerTests
    {
        private readonly Mock<IPropertyService> _mockPropertyService;
        private readonly Mock<ILogger<PropertiesController>> _mockLogger;
        private readonly PropertiesController _controller;

        public PropertiesControllerTests()
        {
            _mockPropertyService = new Mock<IPropertyService>();
            _mockLogger = new Mock<ILogger<PropertiesController>>();
            _controller = new PropertiesController(_mockPropertyService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProperties_WhenServiceReturnsData_ReturnsOkResult()
        {
            // Arrange
            var expectedData = new PropertiesResponse
            {
                Properties = new List<Property>
                {
                    new Property { Id = "P101", Name = "Test Property" }
                }
            };
            _mockPropertyService.Setup(s => s.GetAllPropertiesAsync())
                               .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetProperties();

            // Assert
            var okResult = Assert.IsType<ActionResult<PropertiesResponse>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<PropertiesResponse>(okObjectResult.Value);
            Assert.Single(returnValue.Properties);
            Assert.Equal("P101", returnValue.Properties[0].Id);
        }

        [Fact]
        public async Task GetProperties_WhenServiceReturnsNull_ReturnsErrorResponse()
        {
            // Arrange
            _mockPropertyService.Setup(s => s.GetAllPropertiesAsync())
                               .ReturnsAsync((PropertiesResponse?)null);

            // Act
            var result = await _controller.GetProperties();

            // Assert
            var actionResult = Assert.IsType<ActionResult<PropertiesResponse>>(result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            
            var errorResponse = Assert.IsType<ErrorResponse>(statusCodeResult.Value);
            Assert.Equal("Internal Server Error", errorResponse.Title);
            Assert.Equal(500, errorResponse.Status);
        }

        [Fact]
        public async Task GetProperty_WhenPropertyExists_ReturnsOkResult()
        {
            // Arrange
            var propertyId = "P101";
            var expectedProperty = new Property { Id = propertyId, Name = "Test Property" };
            
            _mockPropertyService.Setup(s => s.GetPropertyByIdAsync(propertyId))
                               .ReturnsAsync(expectedProperty);

            // Act
            var result = await _controller.GetProperty(propertyId);

            // Assert
            var okResult = Assert.IsType<ActionResult<Property>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<Property>(okObjectResult.Value);
            Assert.Equal(propertyId, returnValue.Id);
        }

        [Fact]
        public async Task GetProperty_WhenPropertyDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var propertyId = "NONEXISTENT";
            _mockPropertyService.Setup(s => s.GetPropertyByIdAsync(propertyId))
                               .ReturnsAsync((Property?)null);

            // Act
            var result = await _controller.GetProperty(propertyId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Property>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
            
            var errorResponse = Assert.IsType<ErrorResponse>(notFoundResult.Value);
            Assert.Equal("Not Found", errorResponse.Title);
            Assert.Equal(404, errorResponse.Status);
        }

        [Fact]
        public async Task GetPropertySpaces_WhenPropertyExists_ReturnsSpaces()
        {
            // Arrange
            var propertyId = "P101";
            var expectedProperty = new Property 
            { 
                Id = propertyId, 
                Name = "Test Property",
                Spaces = new List<Space>
                {
                    new Space { Id = "S101", Name = "Test Space", Type = "Office", Size = 1000 }
                }
            };
            
            _mockPropertyService.Setup(s => s.GetPropertyByIdAsync(propertyId))
                               .ReturnsAsync(expectedProperty);

            // Act
            var result = await _controller.GetPropertySpaces(propertyId);

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<Space>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<List<Space>>(okObjectResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("S101", returnValue[0].Id);
        }

        [Fact]
        public async Task GetPropertySpaces_WhenPropertyDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var propertyId = "NONEXISTENT";
            _mockPropertyService.Setup(s => s.GetPropertyByIdAsync(propertyId))
                               .ReturnsAsync((Property?)null);

            // Act
            var result = await _controller.GetPropertySpaces(propertyId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Space>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
