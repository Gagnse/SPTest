// frontend/src/components/WorkspaceNavbar.tsx
import React, { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import '../styles/Workspace/workspaceNavbar.css';

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role?: string;
  organizationName?: string;
}

const WorkspaceNavbar: React.FC = () => {
  const location = useLocation();
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const userInfo = localStorage.getItem('currentUser');
    if (userInfo) {
      setUser(JSON.parse(userInfo));
    }
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    window.location.href = '/login';
  };

  const isActive = (path: string) => location.pathname === path;

  return (
    <nav className="workspace-navbar">
      <div className="workspace-navbar-container">
        <div className="workspace-navbar-left">
          {/* Logo */}
          <div className="workspace-logo">
            <Link to="/workspace/myprojects">SpaceLogic</Link>
          </div>
          
          {/* Navigation Items - Will be filled later */}
          <div className="workspace-nav-links">
            {/* Navigation items will be added here later */}
          </div>
        </div>

        <div className="workspace-navbar-right">
          {/* User Info */}
          {user && (
            <div className="user-info">
              <span className="welcome-text">
                Bonjour, {user.firstName}
              </span>
              {user.organizationName && (
                <span className="organization-name">
                  • {user.organizationName}
                </span>
              )}
            </div>
          )}

          {/* Logout Button */}
          <button className="logout-btn" onClick={handleLogout}>
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
              <polyline points="16 17 21 12 16 7"></polyline>
              <line x1="21" y1="12" x2="9" y2="12"></line>
            </svg>
            Déconnexion
          </button>
        </div>
      </div>

      {/* Secondary Navigation */}
      <div className="workspace-secondary-nav">
        <div className="workspace-secondary-nav-container">
          <div className="secondary-nav-items">
            <Link 
              to="/workspace/myprojects" 
              className={`secondary-nav-item ${isActive('/workspace/myprojects') ? 'active' : ''}`}
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path>
              </svg>
              <span>Mes projets</span>
            </Link>
            {/* More navigation items will be added here later */}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default WorkspaceNavbar;