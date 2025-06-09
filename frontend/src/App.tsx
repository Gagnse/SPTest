import { useEffect } from 'react';
import { BrowserRouter as Router, useRoutes } from 'react-router-dom';
import axios from 'axios';
import publicRoutes from './routes/publicRoutes';
import Navbar from './components/Navbar'; // ✅ Ajout de la navbar
import Footer from './components/Footer'; // ✅ Ajout du footer

function RoutesWrapper() {
  const routing = useRoutes(publicRoutes);
  return routing;
}

function App() {

  useEffect(() => {
    axios
      .get(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/weatherforecast`)
      .then(res => {
        console.log('Réponse de l’API :', res.data);
      })
      .catch(err => {
        console.error('Erreur :', err);
      });
  }, []);

  return (
    <Router>
      <Navbar />              {/* ✅ Navbar visible sur toutes les pages */}
      <RoutesWrapper />       {/* ✅ Affiche le contenu des routes */}
      <Footer />              {/* ✅ Footer visible sur toutes les pages */}
    </Router>
  );
}

export default App;
