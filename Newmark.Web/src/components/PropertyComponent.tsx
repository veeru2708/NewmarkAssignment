import React, { useState } from 'react';
import { Property } from '../services/apiService';
import SpaceComponent from './SpaceComponent';
import './PropertyComponent.css';

interface PropertyComponentProps {
  property: Property;
}

const PropertyComponent: React.FC<PropertyComponentProps> = ({ property }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  const toggleExpanded = () => {
    setIsExpanded(!isExpanded);
  };

  return (
    <div className="property-component">
      {/* Property Header */}
      <div 
        className={`property-header ${isExpanded ? 'expanded' : ''}`}
        onClick={toggleExpanded}
      >
        <div className="property-header-content">
          <div className="property-info">
            <h2 className="property-title">
              {property.PropertyName}
            </h2>
            <p className="property-id">
              Property ID: {property.PropertyId}
            </p>
            
            {/* Basic counts from raw data - not calculated metrics */}
            <div className="property-counts">
              <div className="count-item">
                <div className="count-label">Spaces</div>
                <div className="count-value">
                  {property.Spaces?.length || 0}
                </div>
              </div>
              <div className="count-item">
                <div className="count-label">Transit Options</div>
                <div className="count-value">
                  {property.Transportation?.length || 0}
                </div>
              </div>
              {/* Note: Monthly Revenue would be calculated from rent roll data */}
            </div>
          </div>
          
          <div className="expand-icon">
            {isExpanded ? '▼' : '▶'}
          </div>
        </div>
      </div>

      {isExpanded && (
        <div className="property-details">
          {/* Property Overview Grid */}
          <div className="details-grid">
            
            {/* Features Section */}
            {property.Features && property.Features.length > 0 && (
              <div className="detail-section">
                <h3 className="section-title features">
                  Features
                </h3>
                <ul className="detail-list">
                  {property.Features.map((feature, index) => (
                    <li key={index} className="detail-item">
                      {feature}
                    </li>
                  ))}
                </ul>
              </div>
            )}

            {/* Highlights Section */}
            {property.Highlights && property.Highlights.length > 0 && (
              <div className="detail-section">
                <h3 className="section-title highlights">
                  Key Highlights
                </h3>
                <ul className="detail-list">
                  {property.Highlights.map((highlight, index) => (
                    <li key={index} className="detail-item">
                      {highlight}
                    </li>
                  ))}
                </ul>
              </div>
            )}

            {/* Transportation Section */}
            {property.Transportation && property.Transportation.length > 0 && (
              <div className="detail-section">
                <h3 className="section-title transportation">
                  Transportation Access
                </h3>
                <div className="transport-grid">
                  {property.Transportation.map((transport, index) => (
                    <div key={index} className="transport-item">
                      <div className="transport-info">
                        <div className="transport-type">
                          {transport.Type}
                        </div>
                        <div className="transport-details">
                          {transport.Line && `${transport.Line} `}
                          {transport.Station && `(${transport.Station})`}
                        </div>
                      </div>
                      <div className="transport-distance">
                        {transport.Distance}
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>

          {/* Spaces Section */}
          {property.Spaces && property.Spaces.length > 0 && (
            <div className="spaces-section">
              <div className="spaces-header">
                <span className="spaces-icon">🏢</span>
                <h3 className="spaces-title">
                  Rental Spaces ({property.Spaces.length})
                </h3>
              </div>
              <div className="spaces-list">
                {property.Spaces.map((space) => (
                  <SpaceComponent key={space.SpaceId} space={space} />
                ))}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default PropertyComponent;
