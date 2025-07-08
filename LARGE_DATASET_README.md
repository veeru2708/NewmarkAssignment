# Large Dataset Handling Feature

This branch contains optimizations for handling very large Azure Blob Storage files (100MB+) in the Newmark Real Estate Portfolio application.

## Features Added

### 1. OptimizedBlobPropertyRepository
- **Streaming Downloads**: Uses `DownloadStreamingAsync()` for files > 10MB
- **Memory Caching**: Implements `IMemoryCache` with configurable expiry
- **Size Limits**: Configurable maximum download size (default: 200MB)
- **Timeout Handling**: Configurable download timeouts (default: 10 minutes)
- **Performance Monitoring**: Detailed logging of download sizes and times

### 2. LargeDatasetServiceExtensions
- **Service Registration**: Extension methods for registering optimized services
- **Kestrel Configuration**: Increased request size limits and timeouts
- **Health Checks**: Memory usage and repository health monitoring
- **Caching Configuration**: Optimized memory cache settings

### 3. Configuration Files
- **appsettings.LargeDataset.json**: Specific settings for large dataset scenarios
- **Configurable Limits**: All size limits, timeouts, and cache settings are configurable

## Configuration Options

### Azure Blob Settings
```json
{
  "AzureBlob": {
    "MaxDownloadSizeMB": 200,        // Maximum blob size to download
    "DownloadTimeoutMinutes": 10     // Timeout for download operations
  }
}
```

### Caching Settings
```json
{
  "Caching": {
    "MaxCacheSizeMB": 100,           // Maximum memory cache size
    "CacheExpiryMinutes": 30         // Cache expiration time
  }
}
```

### Kestrel Server Settings
```json
{
  "Kestrel": {
    "MaxRequestSizeMB": 250,         // Maximum request body size
    "KeepAliveTimeoutMinutes": 15,   // Keep-alive timeout
    "RequestTimeoutMinutes": 5,      // Request header timeout
    "MaxConnections": 1000,          // Maximum concurrent connections
    "MaxUpgradedConnections": 1000   // Maximum upgraded connections
  }
}
```

## Usage

### To Use Large Dataset Optimizations

1. **Update Program.cs** to use the optimized services:
```csharp
// Replace the standard service registration with:
builder.Services.AddLargeDatasetServices(builder.Configuration);
builder.Services.ConfigureKestrelForLargePayloads(builder.Configuration);
builder.Services.AddLargeDatasetHealthChecks();
```

2. **Use the LargeDataset configuration**:
```csharp
// In Program.cs, load the large dataset configuration
builder.Configuration.AddJsonFile("appsettings.LargeDataset.json", optional: true, reloadOnChange: true);
```

### Performance Characteristics

#### Small Files (< 10MB)
- **Method**: Direct download with `DownloadContentAsync()`
- **Memory Usage**: Loads entire file into memory
- **Performance**: Fastest for small files

#### Large Files (10MB - 200MB)
- **Method**: Streaming download with `DownloadStreamingAsync()`
- **Memory Usage**: Minimal - streams directly to JSON deserializer
- **Performance**: Memory efficient, slightly slower due to streaming

#### Very Large Files (> 200MB)
- **Behavior**: Rejects download with clear error message
- **Reason**: Prevents out-of-memory exceptions
- **Configurable**: Limit can be adjusted via configuration

### Caching Strategy

1. **First Request**: Downloads and caches data for 30 minutes (configurable)
2. **Subsequent Requests**: Served from memory cache
3. **Cache Miss**: Re-downloads and re-caches data
4. **Manual Cache Clear**: Available via `ClearCache()` method

### Monitoring and Logging

The optimized repository provides detailed logging:
- Download sizes and timing
- Cache hit/miss statistics
- Memory usage monitoring
- Error details and fallback scenarios

### Health Checks

Available health check endpoints:
- `/health/large-dataset` - Repository connectivity and size limits
- `/health/memory` - Current memory usage monitoring

## When to Use This Branch

### Use Large Dataset Optimizations When:
- Azure blob files are consistently > 50MB
- Memory usage is a concern
- Download timeouts are occurring
- Multiple concurrent users accessing large datasets
- Production deployment with resource constraints

### Use Main Branch When:
- Azure blob files are < 50MB
- Development/testing scenarios
- Simple deployment requirements
- Single-user scenarios

## Testing Large Dataset Scenarios

### Simulating Large Files
1. **Configure smaller limits** for testing:
```json
{
  "AzureBlob": {
    "MaxDownloadSizeMB": 1,          // Test with 1MB limit
    "DownloadTimeoutMinutes": 1      // Test with 1 minute timeout
  }
}
```

2. **Monitor performance** with logging:
```csharp
_logger.LogInformation("Download completed in {ElapsedMs}ms for {SizeMB:F2}MB file", stopwatch.ElapsedMilliseconds, sizeInMB);
```

### Load Testing
The optimized repository includes monitoring for:
- Concurrent download handling
- Memory pressure under load
- Cache efficiency under high traffic
- Graceful degradation when limits are exceeded

## Migration from Main Branch

To migrate existing code to use large dataset optimizations:

1. **Update service registration** in Program.cs
2. **Add configuration** settings to appsettings.json
3. **No other code changes required** - the interface remains the same

The `IPropertyRepository` interface is unchanged, so all existing controller and service code continues to work without modification.

## Future Enhancements

Potential additional optimizations for this branch:
- **Distributed Caching**: Redis cache for multi-instance deployments
- **Background Refresh**: Proactive cache warming
- **Compression**: Gzip decompression for compressed blobs
- **Partial Downloads**: Range requests for specific data subsets
- **CDN Integration**: Content delivery network for global distribution
