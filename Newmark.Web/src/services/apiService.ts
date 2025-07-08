// API service layer for centralized API calls


export interface TransportationInfo {
  Type: string;
  Line: string;
  Station?: string;
  Distance: string;
}

export interface RentRoll {
  Month: string;
  year: number;
  Rent: number;
}

export interface Space {
  SpaceId: string;
  SpaceName: string;
  type: string;
  size: number;
  RentRoll: RentRoll[];
}

export interface Property {
  PropertyId: string;
  PropertyName: string;
  address: string;
  Features: string[];
  Highlights: string[];
  Transportation: TransportationInfo[];
  Spaces: Space[];
}

export interface PropertiesResponse {
  properties: Property[];
}

class ApiService {
  private baseUrl: string;

  constructor() {
    this.baseUrl = process.env.REACT_APP_API_URL || 'https://localhost:7209';
  }

  async getProperties(): Promise<PropertiesResponse> {
    const response = await fetch(`${this.baseUrl}/api/properties`);
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    return response.json();
  }
}

export const apiService = new ApiService();
