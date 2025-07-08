import React from 'react';
import './App.css';
import PropertiesContainer from './components/PropertiesContainer';

function App() {
  return (
    <div className="app-container">
      <header className="app-header">
        <div className="app-header-content">
          <h1 className="app-title">
            Property Management System
          </h1>
        </div>
      </header>

      <main className="app-main">
        <PropertiesContainer />
      </main>
    </div>
  );
}

export default App;
