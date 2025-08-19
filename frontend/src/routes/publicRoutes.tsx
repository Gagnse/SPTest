// frontend/src/routes/publicRoutes.tsx
import Home from '../pages/Home';
import About from '../pages/About';
import Services from '../pages/Services';
import Contact from '../pages/Contact';
import Login from '../pages/auth/Login';
import PrivacyPolicy from '../pages/PrivacyPolicy';
import TermsOfService from '../pages/TermsOfService';
import ProjectsPage from '../pages/workspace/ProjectsPage';
import AdministrationPage from '../pages/workspace/AdministrationPage';
import UsersPage from '../pages/workspace/UsersPage';
import ActivitiesPage from '../pages/workspace/ActivitiesPage';

const publicRoutes = [
  { path: '/', element: <Home /> },
  { path: '/about', element: <About /> },
  { path: '/services', element: <Services /> },
  { path: '/contact', element: <Contact /> },
  { path: '/login', element: <Login /> },
  { path: '/privacy', element: <PrivacyPolicy /> },
  { path: '/terms', element: <TermsOfService />},
  
  // Workspace routes
  { path: '/workspace/myprojects', element: <ProjectsPage /> },
  { path: '/workspace/administration', element: <AdministrationPage /> },
  { path: '/workspace/users', element: <UsersPage /> },
  { path: '/workspace/activities', element: <ActivitiesPage /> },
  
  // French routes
  { path: '/projets', element: <ProjectsPage /> }
];

export default publicRoutes;