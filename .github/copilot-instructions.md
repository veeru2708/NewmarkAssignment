<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# Full-Stack Application: .NET Web API + React TypeScript

This workspace contains a full-stack web application with:
- **Backend**: .NET 8 Web API (in `backend/` folder)
- **Frontend**: React TypeScript application (in `frontend/` folder)

## Architecture Guidelines

### Backend (.NET Web API)
- Use minimal APIs for simple endpoints
- Follow RESTful conventions
- Use dependency injection for services
- Implement proper error handling and logging
- Use Data Transfer Objects (DTOs) for API responses
- Configure CORS for frontend communication

### Frontend (React TypeScript)
- Use functional components with hooks
- Implement proper TypeScript interfaces for API responses
- Use async/await for API calls
- Handle loading states and error scenarios
- Follow React best practices and conventions

### API Communication
- Backend runs on `https://localhost:7209` (HTTPS)
- Frontend runs on `http://localhost:3000` 
- CORS is configured to allow frontend-backend communication
- Use proper HTTP status codes and error handling

## Development Guidelines
- Keep backend and frontend code separate
- Use TypeScript for type safety in frontend
- Follow C# naming conventions in backend
- Use meaningful component and service names
- Implement proper error boundaries and validation
