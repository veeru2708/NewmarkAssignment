using Newmark.API.Models;
using Newmark.API.Services;
using Newmark.API.Services.IServices;
using Newmark.API.Repositories;
using Newmark.API.Repositories.IRepositories;
using Newmark.API.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.Information);
}

// Add services to the container.
builder.Services.AddControllers();

// Add model validation
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
    .AddCheck("blob_storage", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()); // TODO: Add actual blob storage health check

// Configure Swagger/OpenAPI for comprehensive API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Newmark Real Estate Portfolio API",
        Version = "v1",
        Description = "RESTful Web API for managing real estate property portfolio data with Azure Blob Storage integration. Follows clean architecture principles with proper separation of concerns.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Newmark Technical Team",
            Email = "tech@newmark.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Include XML comments for comprehensive API documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register application services and repositories with dependency injection
// Repository layer for data access
builder.Services.AddScoped<IPropertyRepository, BlobPropertyRepository>();

// Service layer for business logic
builder.Services.AddScoped<IPropertyService, PropertyService>();

// Configure CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:3000", "https://localhost:3000") // React dev server (HTTP and HTTPS)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Newmark Real Estate Portfolio API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Newmark API Documentation";
        c.DefaultModelsExpandDepth(-1); // Hide models section by default
    });
    
    // Add request/response logging in development
    app.UseRequestResponseLogging();
}

// Use CORS before other middleware
app.UseCors("AllowReactApp");

// Security headers
app.UseHsts();
app.UseHttpsRedirection();

// Authentication and Authorization (placeholder for future implementation)
app.UseAuthentication();
app.UseAuthorization();

// Map Health Checks endpoint
app.MapHealthChecks("/health")
    .WithDisplayName("Application Health Check")
    .WithTags("Health");

// Map API controllers
app.MapControllers()
    .WithDisplayName("Property Management API")
    .WithTags("API");

app.Run();
