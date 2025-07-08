# Newmark Real Estate Portfolio Application

A modern full-stack web application built with .NET 8 Web API backend and React TypeScript frontend that integrates with Azure Blob Storage to display property portfolio data.

## 🎉 Status: Fully Functional and Tested

✅ **Azure Blob Storage**: Successfully connected and retrieving live data  
✅ **Backend API**: Running on `https://localhost:7209` with proper JSON deserialization  
✅ **Frontend UI**: Running on `http://localhost:3000` with hierarchical property display  
✅ **Data Structure**: Updated to match the latest blob JSON format  
✅ **Transportation**: Supports all types (Subway, Bus, Train, Bike Share) with Line/Station/Distance  
✅ **Swagger UI**: Available at `https://localhost:7209/swagger` for API documentation  

**Current Data**: Successfully displaying properties P101 "The Grand Plaza" and P102 "Ocean View Tower" with full features, highlights, transportation, spaces, and rent roll data.

## Overview

This application demonstrates:
- **Azure Blob Storage Integration**: Retrieves JSON data from Azure Blob Storage using SAS token authentication
- **Data Transformation**: Converts hierarchical JSON data into structured C# models
- **RESTful API**: Exposes property data through a clean API endpoint
- **React Frontend**: Displays properties with collapsible parent-child structure
- **Error Handling**: Robust error handling throughout the application stack

## Project Structure

```
├── backend/                    # .NET 8 Web API
│   ├── Models/                 # Property data models
│   ├── Services/               # Azure Blob Storage service
│   └── Program.cs              # API endpoints and configuration
├── frontend/                   # React TypeScript App
│   ├── src/components/         # React components
│   ├── src/services/           # API service layer
│   └── src/App.tsx             # Main application
├── .github/                    # GitHub Copilot instructions
└── .vscode/                    # VS Code tasks and settings
```

## Features

### Backend (.NET 8 Web API)
- **Azure Blob Storage Integration**: Connects to Azure using SAS token authentication
- **Data Models**: Property, Space, RentRoll, and TransportationInfo models
- **API Endpoint**: `/api/properties` returns structured property data
- **Error Handling**: Graceful fallback to mock data if Azure access fails
- **Async Operations**: Efficient handling of large datasets
- **CORS Configuration**: Enabled for frontend communication

### Frontend (React TypeScript)
- **Hierarchical Display**: Property → Spaces → RentRoll structure
- **Collapsible Sections**: Expandable property and space details
- **Type Safety**: Full TypeScript interfaces for all data models
- **Error Boundaries**: Comprehensive error handling and user feedback
- **Responsive Design**: Modern, clean UI with proper styling
- **Loading States**: User-friendly loading indicators

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

### Quick Start (Recommended)

Use the VS Code task to start both backend and frontend simultaneously:

1. Open VS Code
2. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
3. Type "Tasks: Run Task"
4. Select "Start Full Stack App"

This will start both the backend API and frontend development server.

### Manual Start

#### Backend API

```bash
cd backend
dotnet run
```

The API will be available at: `https://localhost:7209` (HTTPS)

#### Frontend

```bash
cd frontend
npm start
```

The React app will be available at: `http://localhost:3000`

## API Endpoints

### Properties API

- **GET** `/api/properties`
- **Description**: Retrieves property portfolio data from Azure Blob Storage
- **Authentication**: Uses SAS token configured in appsettings.json
- **Response**: JSON object containing array of properties with spaces and rent roll data
- **Error Handling**: Returns HTTP 500 with error details if retrieval fails

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

## Building for Production

### Backend

```bash
cd backend
dotnet publish -c Release
```

### Frontend

```bash
cd frontend
npm run build
```

## Troubleshooting

### CORS Issues

If you encounter CORS errors:
1. Ensure the backend is running on `https://localhost:7209`
2. Check that CORS is configured correctly in `backend/Program.cs`

### Port Conflicts

If ports are already in use:
- Backend: Modify `launchSettings.json` in `backend/Properties/`
- Frontend: Set the `PORT` environment variable or modify package.json

### SSL Certificate Issues

For HTTPS development certificate:
```bash
dotnet dev-certs https --trust
```

## Technologies Used

- **.NET 8**: Backend API framework
- **React 18**: Frontend library
- **TypeScript**: Type-safe JavaScript
- **Minimal APIs**: Lightweight API endpoints
- **Create React App**: React project tooling

## Next Steps

- Add authentication and authorization
- Implement data persistence with Entity Framework
- Add more API endpoints and React components
- Set up automated testing
- Configure CI/CD pipeline
