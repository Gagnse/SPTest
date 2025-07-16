// frontend/src/App.tsx
import { useEffect } from 'react';
import { BrowserRouter as Router, useRoutes, useLocation } from 'react-router-dom';
import axios from 'axios';
import publicRoutes from './routes/publicRoutes';
import Navbar from './components/Navbar';
import Footer from './components/Footer';

function RoutesWrapper() {
  const routing = useRoutes(publicRoutes);
  const location = useLocation();
  
  // Check if current route is a workspace route
  const isWorkspaceRoute = location.pathname.startsWith('/workspace');
  
  return (
    <>
      {/* Only show main navbar on non-workspace pages */}
      {!isWorkspaceRoute && <Navbar />}
      
      {/* Main content */}
      {routing}
      
      {/* Only show footer on non-workspace pages */}
      {!isWorkspaceRoute && <Footer />}
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