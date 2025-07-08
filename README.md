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


### Available Scripts (Newmark.Web)

In the Newmark.Web directory, you can run:

#### `npm start`
Runs the React app in development mode at [http://localhost:3000](http://localhost:3000).
The page reloads automatically when you make edits, and lint errors appear in the console.


