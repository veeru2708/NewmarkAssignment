<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# Full-Stack Application: .NET Web API + React TypeScript

This workspace contains a full-stack web application with:
- **Backend**: .NET 8 Web API (Newmark.API folder)
- **Frontend**: React TypeScript application (Newmark.Web folder)

## Quick Reference

📖 **Complete Documentation**: See the main [README.md](../README.md) for comprehensive setup, architecture, and development guidelines.

📋 **Architecture Summary**: See [REPOSITORY_PATTERN_SUMMARY.md](../REPOSITORY_PATTERN_SUMMARY.md) for detailed implementation patterns.

## Key Architecture Guidelines

### Backend (Newmark.API - .NET Web API)
- **Repository Pattern**: Use IPropertyRepository and IPropertyService interfaces
- **Clean Architecture**: Separation of concerns with repository, service, and controller layers
- **Dependency Injection**: Scoped services for all layers
- **RESTful Conventions**: Proper HTTP status codes and error responses
- **Structured Logging**: Comprehensive logging throughout all operations
- **Error Handling**: Graceful fallback with detailed error information

### Frontend (Newmark.Web - React TypeScript)  
- **Custom CSS**: Use component-specific CSS files - no external frameworks
- **Functional Components**: React hooks with proper TypeScript interfaces
- **Async/Await**: Proper error handling for all API calls
- **Loading States**: User-friendly indicators and state management
- **Component Architecture**: Hierarchical display patterns

### API Communication
- **Backend**: `https://localhost:7209` (HTTPS) with CORS configuration
- **Frontend**: `http://localhost:3000` (HTTP) with structured error handling
- **Data Flow**: Repository → Service → Controller → Frontend Service → Components

### Development Standards
- **Backend**: Follow C# naming conventions and dependency injection patterns
- **Frontend**: Use meaningful component names with TypeScript interfaces
- **Testing**: Mock-based unit testing for all service layers
- **Documentation**: Inline comments and comprehensive error messages

For complete setup instructions, API documentation, troubleshooting, and architectural details, refer to the main [README.md](../README.md) file.
