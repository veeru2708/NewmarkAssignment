import React, { useState, useEffect } from 'react';
import { apiService, PropertiesResponse } from '../services/apiService';
import PropertyComponent from './PropertyComponent';
import './PropertiesContainer.css';

const PropertiesContainer: React.FC = () => {
  const [propertiesData, setPropertiesData] = useState<PropertiesResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchPropertiesData = async () => {
      try {
        setLoading(true);
        const data = await apiService.getProperties();
        setPropertiesData(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
        console.error('Error fetching properties:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchPropertiesData();
  }, []);

  if (loading) {
    return (
      <div className="loading-container">
        <div className="loading-icon">🏢</div>
        Loading properties data...
      </div>
    );
  }

  if (error) {
    return (
      <div className="error-container">
        <h3 className="error-title">Error Loading Properties</h3>
        <p className="error-message">{error}</p>
        <p className="error-hint">
          Make sure the backend API is running on {process.env.REACT_APP_API_URL || 'https://localhost:7209'}
        </p>
      </div>
    );
  }

  if (!propertiesData || !propertiesData.properties || propertiesData.properties.length === 0) {
    return (
      <div className="no-data-container">
        <h3 className="no-data-title">No Properties Found</h3>
        <p className="no-data-message">No property data is currently available.</p>
      </div>
    );
  }

  return (
    <div className="properties-container">
      {/* Header Section */}
      <div className="header-section">
        <h1 className="header-title">
          Property Portfolio
        </h1>
        
        <div className="metrics-container">
          {/* Note: These summary statistics would be calculated from the property data in a real application */}
          {/* Total Properties count - calculated from data */}
          <div className="metric-card">
            <div className="metric-label">Total Properties</div>
            <div className="metric-value properties">
              {propertiesData.properties.length}
            </div>
          </div>
          
          {/* Total Spaces count - calculated from data */}
          <div className="metric-card">
            <div className="metric-label">Total Spaces</div>
            <div className="metric-value spaces">
              {propertiesData.properties.reduce((total, prop) => total + (prop.Spaces?.length || 0), 0)}
            </div>
          </div>
          
          {/* Annual Revenue - calculated from rent roll data */}
          <div className="metric-card wide">
            <div className="metric-label">Annual Revenue</div>
            <div className="metric-value revenue">
              {new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD',
                minimumFractionDigits: 0,
                maximumFractionDigits: 0
              }).format(
                propertiesData.properties.reduce((total, prop) => {
                  return total + (prop.Spaces?.reduce((spaceTotal, space) => {
                    const latestRent = space.RentRoll && space.RentRoll.length > 0 
                      ? space.RentRoll[space.RentRoll.length - 1].Rent 
                      : 0;
                    return spaceTotal + (latestRent * 12); // Annual value
                  }, 0) || 0);
                }, 0)
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Properties List */}
      <div className="properties-list">
        {propertiesData.properties.map((property) => (
          <PropertyComponent key={property.PropertyId} property={property} />
        ))}
      </div>
    </div>
  );
};

export default PropertiesContainer;
