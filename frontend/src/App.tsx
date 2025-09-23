// frontend/src/App.tsx
import { useEffect, useState } from 'react';
import { BrowserRouter as Router, useRoutes, useLocation } from 'react-router-dom';
import axios from 'axios';
import publicRoutes from './routes/publicRoutes';
import Navbar from './components/Navbar';
import Footer from './components/Footer';

function RoutesWrapper() {
  const routing = useRoutes(publicRoutes);
  const location = useLocation();
  const [isNavigating] = useState(false);
  const [fadeOpacity] = useState(1);
  
  // Check if current route is a workspace route
  const isWorkspaceRoute = location.pathname.startsWith('/workspace');
  
  return (
    <>
      {/* Main navbar with smooth fade */}
      <div style={{ 
        opacity: (!isWorkspaceRoute && !isNavigating) ? fadeOpacity : 0,
        transition: 'opacity 0.15s ease-in-out'
      }}>
        {!isWorkspaceRoute && <Navbar key="main-navbar" />}
      </div>
      
      {/* Main content with smooth fade */}
      <div 
        key={location.pathname}
        style={{ 
          opacity: fadeOpacity,
          transition: 'opacity 0.15s ease-in-out'
        }}
      >
        {routing}
      </div>
      
      {/* Footer with smooth fade */}
      <div style={{ 
        opacity: (!isWorkspaceRoute && !isNavigating) ? fadeOpacity : 0,
        transition: 'opacity 0.15s ease-in-out'
      }}>
        {!isWorkspaceRoute && <Footer key="main-footer" />}
      </div>
    </>
  );
}

function App() {
  useEffect(() => {
    axios
      .get(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/weatherforecast`)
      .then(res => {
        console.log("Réponse de l'API :", res.data);
      })
      .catch(err => {
        console.error('Erreur :', err);
      });
  }, []);

  return (
    <Router>
      <RoutesWrapper />
    </Router>
  );
}

export default App;