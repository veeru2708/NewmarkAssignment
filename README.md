# Newmark Real Estate Portfolio Application

A modern full-stack web application built with .NET 8 Web API backend and React TypeScript frontend that integrates with Azure Blob Storage to display property portfolio data using clean architecture and the repository pattern.

## 🎉 Status: Fully Functional and Tested

✅ **Azure Blob Storage**: Successfully connected and retrieving live data  
✅ **Backend API**: Running on `https://localhost:7209` with proper JSON deserialization  
✅ **Frontend UI**: Running on `http://localhost:3000` with hierarchical property display  
✅ **Data Structure**: Updated to match the latest blob JSON format  
✅ **Transportation**: Supports all types (Subway, Bus, Train, Bike Share) with Line/Station/Distance  
✅ **Swagger UI**: Available at `https://localhost:7209/swagger` for API documentation  
✅ **Repository Pattern**: Clean architecture with IPropertyRepository and IPropertyService interfaces  
✅ **Unit Tests**: Comprehensive testing with mocking for better test isolation  

**Current Data**: Successfully displaying properties P101 "The Grand Plaza" and P102 "Ocean View Tower" with full features, highlights, transportation, spaces, and rent roll data.

## Architecture & Development Guidelines

### Backend (.NET 8 Web API) - Newmark.API
- **Clean Architecture**: Repository pattern with IPropertyRepository and IPropertyService interfaces
- **Dependency Injection**: Scoped services for data access and business logic layers
- **RESTful Conventions**: Proper HTTP status codes and error handling
- **Structured Logging**: Comprehensive logging throughout the application
- **Error Handling**: Graceful fallback to mock data with proper error responses
- **Azure Integration**: Blob Storage service with SAS token authentication
- **Unit Testing**: Mock-based testing for service and repository layers

### Frontend (React TypeScript) - Newmark.Web  
- **Custom CSS Styling**: No external CSS frameworks - pure custom CSS for full control
- **Functional Components**: React hooks with proper TypeScript interfaces
- **Async Operations**: Proper async/await patterns for API calls
- **Error Boundaries**: Comprehensive error handling and user feedback
- **Loading States**: User-friendly loading indicators and state management
- **Component Architecture**: Hierarchical display with collapsible sections

### API Communication
- **Backend**: `https://localhost:7209` (HTTPS) with CORS configuration
- **Frontend**: `http://localhost:3000` (HTTP) with proper error handling
- **Data Flow**: Repository → Service → Controller → Frontend Service → Components

## Overview

This application demonstrates:
- **Azure Blob Storage Integration**: Retrieves JSON data from Azure Blob Storage using SAS token authentication
- **Data Transformation**: Converts hierarchical JSON data into structured C# models
- **RESTful API**: Exposes property data through a clean API endpoint
- **React Frontend**: Displays properties with collapsible parent-child structure
- **Error Handling**: Robust error handling throughout the application stack

## Project Structure

```
├── Newmark.API/                # .NET 8 Web API (Backend)
│   ├── Controllers/             # API Controllers (PropertiesController)
│   ├── Models/                  # Data models (Property, Space, RentRoll, etc.)
│   ├── Services/                # Business logic layer (IPropertyService, PropertyService)
│   ├── Repositories/            # Data access layer (IPropertyRepository, BlobPropertyRepository)
│   ├── Middleware/              # Request/Response logging middleware
│   └── Program.cs               # Application configuration and DI setup
├── Newmark.API.Tests/          # Unit tests for the API
│   └── Controllers/             # Controller tests with service mocking
├── Newmark.Web/                # React TypeScript App (Frontend)
│   ├── src/components/          # React components (Property, Space, Container)
│   ├── src/services/            # API service layer for backend communication
│   ├── src/*.css                # Custom CSS styling (no external frameworks)
│   └── src/App.tsx              # Main application component
├── .github/                    # GitHub configuration
├── .vscode/                    # VS Code tasks and settings
├── REPOSITORY_PATTERN_SUMMARY.md # Architecture documentation
└── NewmarkAssignment.sln       # Visual Studio solution file
```

## Features

### Backend (Newmark.API - .NET 8 Web API)
- **Repository Pattern**: Clean separation with IPropertyRepository interface and BlobPropertyRepository implementation
- **Service Layer**: IPropertyService and PropertyService for business logic orchestration  
- **Azure Blob Storage Integration**: Connects to Azure using SAS token authentication
- **Data Models**: Property, Space, RentRoll, and TransportationInfo models with proper validation
- **API Endpoints**: RESTful `/api/properties` endpoints with comprehensive error handling
- **Fallback Strategy**: Graceful fallback to mock data if Azure access fails
- **Async Operations**: Efficient async/await patterns for large datasets
- **CORS Configuration**: Properly configured for frontend communication
- **Dependency Injection**: Scoped services registered for repository and service layers
- **Health Checks**: Service and repository health monitoring
- **Structured Logging**: Comprehensive logging throughout all layers

### Frontend (Newmark.Web - React TypeScript)
- **Custom CSS Architecture**: Pure custom CSS files - no external frameworks like Bootstrap or Material-UI
- **Component Structure**: Hierarchical Property → Spaces → RentRoll display architecture
- **Collapsible Sections**: Interactive expandable property and space details
- **Type Safety**: Complete TypeScript interfaces for all API response models
- **Error Boundaries**: Comprehensive error handling with user-friendly feedback
- **Loading States**: Professional loading indicators and state management
- **Responsive Design**: Modern, clean UI with consistent styling patterns
- **API Integration**: Dedicated service layer for backend communication
- **CSS Organization**: Component-specific CSS files (PropertyComponent.css, SpaceComponent.css, etc.)

### Styling Approach
- **No External CSS Libraries**: The application uses **custom CSS** instead of frameworks like TailwindCSS, Bootstrap, or Material-UI
- **Component-Based Styling**: Each React component has its own dedicated CSS file
- **Design System**: Consistent color scheme with blue theme (#007acc, #0066aa) and professional styling
- **Modern CSS**: Flexbox layouts, responsive design, and clean typography
- **Note**: While TailwindCSS appears in package-lock.json, it's only there as a transitive dependency of react-scripts and is not used in the application

### Data Structure

The application handles the following hierarchical data:

```
Property
├── Basic Info (name, address)
├── Features (array of amenities)
├── Highlights (array of selling points)
├── Transportation (array of transit options)
└── Spaces (array of rental spaces)
    └── RentRoll (monthly rent history)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (v18 or later)
- [Visual Studio Code](https://code.visualstudio.com/)

## Getting Started

### Quick Start (Recommended - VS Code Tasks)

Use the pre-configured VS Code tasks to start both applications:

#### Method 1: Run Individual Tasks
1. Open VS Code in the project root
2. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
3. Type "Tasks: Run Task" and select:
   - **"Run Newmark API"** - Starts the backend (.NET API)
   - **"Run Newmark Web"** - Starts the frontend (React app)

#### Method 2: Manual Terminal Commands

**Backend API (Newmark.API)**
```powershell
cd Newmark.API
dotnet run
```

**Frontend (Newmark.Web)**
```powershell  
cd Newmark.Web
npm start
```

#### Application URLs
- **Backend API**: `https://localhost:7209` (HTTPS with Swagger UI)
- **Frontend Web**: `http://localhost:3000` (HTTP - opens automatically)

### Available VS Code Tasks
- **Run Newmark API**: Start the .NET Web API server (background task)
- **Run Newmark Web**: Start the React development server (background task)  
- **Build Newmark API**: Build the .NET project
- **Build Newmark Web**: Build the React app for production

### Available Scripts (Newmark.Web)

In the Newmark.Web directory, you can run:

#### `npm start`
Runs the React app in development mode at [http://localhost:3000](http://localhost:3000).
The page reloads automatically when you make edits, and lint errors appear in the console.

#### `npm test`
Launches the test runner in interactive watch mode for React components.

#### `npm run build`
Builds the React app for production to the `build` folder with optimized bundles.

#### `npm run eject`
**Note: This is a one-way operation!** Ejects from Create React App to gain full control over configuration.

## API Endpoints

### Properties API

#### **GET** `/api/properties`
- **Description**: Retrieves all property portfolio data from Azure Blob Storage
- **Authentication**: Uses SAS token configured in appsettings.json  
- **Response**: JSON object containing array of properties with spaces and rent roll data
- **Error Handling**: Returns HTTP 500 with error details if retrieval fails
- **Fallback**: Returns mock data if Azure Blob Storage is unavailable

#### **GET** `/api/properties/{id}`
- **Description**: Retrieves a specific property by ID
- **Parameters**: `id` (string) - Property identifier
- **Response**: Single property object with all related data
- **Error Handling**: Returns HTTP 404 if property not found

#### **GET** `/api/properties/{id}/spaces`  
- **Description**: Retrieves all spaces for a specific property
- **Parameters**: `id` (string) - Property identifier
- **Response**: Array of space objects with rent roll data
- **Error Handling**: Returns HTTP 404 if property not found

### Health Check
#### **GET** `/health`
- **Description**: Checks the health of the API and its dependencies
- **Response**: JSON health status including repository connectivity

**Response Format:**
```json
{
  "properties": [
    {
      "id": "prop1",
      "name": "Downtown Office Complex",
      "address": "123 Main Street, Downtown",
      "features": ["24/7 Security", "Fitness Center"],
      "highlights": ["Downtown location", "LEED certification"],
      "transportation": [
        {
          "line": "Green Line",
          "distance": "0.5 miles"
        }
      ],
      "spaces": [
        {
          "id": "space1",
          "name": "Space A",
          "type": "Office",
          "size": 1500,
          "rentRoll": [
            {
              "month": "January",
              "year": 2024,
              "rent": 1000
            }
          ]
        }
      ]
    }
  ]
}
```

### Weather Forecast

- **GET** `/weatherforecast`
- Returns a 5-day weather forecast
- **Response**: Array of weather forecast objects

## Azure Blob Storage Configuration

### Setup Instructions

The application is pre-configured with the following Azure Blob Storage settings:

- **Blob URL**: `https://nmrkpidev.blob.core.windows.net/dev-test/devtest.json`
- **SAS Token**: Configured in `backend/appsettings.json`
- **Authentication**: Read-only access with expiration date

### Configuration Files

#### Backend Configuration (`backend/appsettings.json`)
```json
{
  "AzureBlob": {
    "BlobUrl": "https://nmrkpidev.blob.core.windows.net/dev-test/devtest.json",
    "SasToken": "?sp=r&st=2024-10-28T10:35:48Z&se=2025-10-28T18:35:48Z&spr=https&sv=2022-11-02&sr=b&sig=..."
  }
}
```

#### Frontend Configuration (`frontend/.env`)
```properties
REACT_APP_API_URL=https://localhost:7209
```

### Error Handling

If Azure Blob Storage is unavailable or the SAS token is expired, the application will:
1. Log the error with detailed information
2. Return mock property data to demonstrate the UI functionality
3. Display appropriate error messages to users

### Mock Data Fallback

When Azure access fails, the application provides sample data including:
- 2 properties (Downtown Office Complex, Suburban Business Park)
- Multiple spaces per property
- Historical rent roll data
- Features, highlights, and transportation information

```typescript
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
```

## Data Integrity & Source

The application strictly displays data from the original Azure Blob Storage JSON with clear separation between raw data and potential calculated metrics:

### Raw Data from Blob Storage (Currently Displayed)
- **Property Information**: PropertyId, PropertyName, Features, Highlights, Transportation
- **Space Information**: SpaceId, SpaceName, RentRoll (Month, Rent amounts)
- **Transportation**: Type, Line, Station, Distance information
- **Rent Roll**: Monthly rent amounts by month name

### Calculated/Derived Data (Commented for Future Implementation)
The following metrics are NOT in the original blob data and are marked with comments indicating they could be calculated:
- **Performance Metrics**: Total revenue, average rent, rent growth trends
- **Analytics**: Rent change percentages, performance summaries
- **Trend Indicators**: Up/down arrows showing rent direction changes
- **Aggregated Statistics**: Property-level and portfolio-level summaries

This approach maintains data integrity while demonstrating where business intelligence features could be added in a production system.

## Development

### VS Code Tasks

- **Run Backend API**: Start the .NET Web API server
- **Run Frontend**: Start the React development server
- **Build Backend**: Build the .NET project
- **Build Frontend**: Build the React app for production
- **Start Full Stack App**: Start both backend and frontend

### Project Configuration

- **Backend Port**: `https://localhost:7209` (HTTPS)
- **Frontend Port**: `http://localhost:3000` (HTTP)
- **CORS**: Configured to allow requests from frontend to backend

### Adding New Features

1. **Backend**: Add new endpoints in `backend/Program.cs`
2. **Frontend**: Create new components in `frontend/src/components/`
3. **Types**: Define TypeScript interfaces for API responses

### Building for Production

#### Backend (Newmark.API)
```powershell
cd Newmark.API  
dotnet publish -c Release -o ./publish
```

#### Frontend (Newmark.Web)
```powershell
cd Newmark.Web
npm run build
```

The build creates a `build` folder with optimized production files ready for deployment.

## Testing

### Unit Tests (Newmark.API.Tests)
```powershell
cd Newmark.API.Tests
dotnet test
```

The test suite includes:
- **Controller Tests**: PropertiesController with mocked services
- **Service Layer Tests**: Business logic validation  
- **Repository Tests**: Data access layer testing with mocks
- **Error Handling Tests**: Comprehensive error scenario coverage

### Frontend Tests (Newmark.Web)
```powershell
cd Newmark.Web  
npm test
```

## Architecture Implementation

### Repository Pattern Benefits
- **Separation of Concerns**: Clear boundaries between data access, business logic, and presentation
- **Testability**: Easy mocking of dependencies for unit testing
- **Maintainability**: Modular design allows for easy modifications and extensions
- **Scalability**: Ready for additional features like caching, different data sources, etc.

### Dependency Injection Setup
```csharp
// Repository layer for data access
builder.Services.AddScoped<IPropertyRepository, BlobPropertyRepository>();

// Service layer for business logic  
builder.Services.AddScoped<IPropertyService, PropertyService>();
```

### Key Architectural Features
- ✅ **Clean Architecture**: Repository and Service patterns
- ✅ **Azure Blob Storage Integration**: With fallback mock data
- ✅ **Comprehensive Error Handling**: Throughout all layers
- ✅ **Structured Logging**: Detailed logging for debugging and monitoring
- ✅ **Unit Testing**: Complete test coverage with mocking
- ✅ **Null Safety**: Proper null handling for robust operation
- ✅ **Dependency Injection**: Best practices implementation

## Troubleshooting

### Common Issues and Solutions

#### CORS Issues
If you encounter CORS errors:
1. Ensure the backend is running on `https://localhost:7209`
2. Check that CORS is configured correctly in `Newmark.API/Program.cs`
3. Verify the frontend is making requests to the correct HTTPS URL

#### Port Conflicts
If ports are already in use:
- **Backend**: Modify `launchSettings.json` in `Newmark.API/Properties/`
- **Frontend**: Set the `PORT` environment variable or modify package.json scripts

#### SSL Certificate Issues
For HTTPS development certificate issues:
```powershell
dotnet dev-certs https --trust
```

#### Azure Blob Storage Connection Issues  
If Azure connectivity fails:
1. Check the SAS token expiration in `appsettings.json`
2. Verify the blob URL is accessible
3. The application will automatically fallback to mock data for development

#### Node.js/NPM Issues
If you encounter npm installation issues:
```powershell  
cd Newmark.Web
Remove-Item node_modules -Recurse -Force
Remove-Item package-lock.json -Force  
npm install
```

#### Build Issues
For .NET build issues:
```powershell
cd Newmark.API
dotnet clean
dotnet restore  
dotnet build
```

### Development Guidelines
- **Backend vs Frontend**: Keep Newmark.API and Newmark.Web code completely separate
- **TypeScript**: Use strict typing for all frontend API interfaces
- **C# Conventions**: Follow standard C# naming conventions in the backend
- **Component Naming**: Use meaningful, descriptive names for React components and services
- **Error Handling**: Implement proper error boundaries and validation throughout
- **CSS Organization**: Keep component-specific styles in separate CSS files

## Technologies Used

### Backend Stack
- **.NET 8**: Latest LTS version for backend API framework
- **Minimal APIs**: Lightweight, performance-focused API endpoints  
- **Azure Blob Storage**: Cloud storage integration with SAS token authentication
- **Dependency Injection**: Built-in DI container for clean architecture
- **xUnit**: Unit testing framework with mocking support

### Frontend Stack  
- **React 18**: Latest stable version with concurrent features
- **TypeScript**: Type-safe JavaScript for better development experience
- **Create React App**: Established tooling for React project setup
- **Custom CSS**: No external frameworks - pure CSS for styling control

### Development Tools
- **Visual Studio Code**: Primary IDE with configured tasks
- **VS Code Tasks**: Pre-configured build and run tasks
- **PowerShell**: Windows shell scripting for automation

## Documentation

- **REPOSITORY_PATTERN_SUMMARY.md**: Detailed architecture implementation guide
- **Swagger UI**: Interactive API documentation at `https://localhost:7209/swagger`
- **Inline Code Comments**: Comprehensive code documentation throughout both projects

This consolidated README provides all the information previously scattered across multiple documentation files, making it the single source of truth for the Newmark Real Estate Portfolio Application.
