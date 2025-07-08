import React, { useState } from 'react';
import { Space } from '../services/apiService';
import './SpaceComponent.css';

interface SpaceComponentProps {
  space: Space;
}

const SpaceComponent: React.FC<SpaceComponentProps> = ({ space }) => {
  const [isExpanded, setIsExpanded] = useState(false);

  const toggleExpanded = () => {
    setIsExpanded(!isExpanded);
  };

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(amount);
  };

  // Get the most recent rent amount from raw data
  const currentRent = space.RentRoll && space.RentRoll.length > 0 
    ? space.RentRoll[space.RentRoll.length - 1].Rent 
    : 0;

  return (
    <div className="space-component">
      <div 
        className={`space-header ${isExpanded ? 'expanded' : ''}`}
        onClick={toggleExpanded}
      >
        <div className="space-header-content">
          <div className="space-info">
            <h4 className="space-title">
              {space.SpaceName}
            </h4>
            <span className="space-id">
              {space.SpaceId}
            </span>
            
            <div className="space-metrics">
              {/* Latest Rent - raw data from RentRoll */}
              <div className="metric-item">
                <span className="metric-label">Latest Rent:</span>
                <span className="metric-value rent">
                  {formatCurrency(currentRent)}
                </span>
              </div>

              {/* Note: Rent trends would be calculated from historical data */}
              {/* Note: Performance metrics would be derived from rent roll analysis */}

              {/* Data Points Count - raw data */}
              <div className="metric-item">
                <span className="metric-label">Data Points:</span>
                <span className="metric-value data-points">
                  {space.RentRoll?.length || 0} months
                </span>
              </div>

              {/* Note: Space type and size are mock data - not in actual blob */}
            </div>
          </div>
          
          <div className="space-expand-icon">
            {isExpanded ? '▼' : '▶'}
          </div>
        </div>
      </div>

      {isExpanded && space.RentRoll && space.RentRoll.length > 0 && (
        <div className="space-details">
          <h5 className="rent-history-title">
            Rent Roll History
          </h5>
          
          <div className="rent-history-list">
            {space.RentRoll.map((rent, index) => {
              const isLatest = index === space.RentRoll!.length - 1;
              // Note: Rent change calculations and trends would be derived from this raw data

              return (
                <div 
                  key={index} 
                  className={`rent-item ${isLatest ? 'latest' : 'regular'}`}
                >
                  <div>
                    <div className={`rent-period ${isLatest ? 'latest' : 'regular'}`}>
                      {rent.Month} {rent.year}
                      {isLatest && (
                        <span className="latest-badge">
                          LATEST
                        </span>
                      )}
                    </div>
                  </div>
                  <div className={`rent-amount ${isLatest ? 'latest' : 'regular'}`}>
                    {formatCurrency(rent.Rent)}
                  </div>
                  {/* Note: Rent change calculations would be shown here */}
                </div>
              );
            })}
          </div>
          
          {/* Note: Summary statistics like total growth, average rent, etc. would be calculated from the raw rent roll data */}
        </div>
      )}
    </div>
  );
};

export default SpaceComponent;
