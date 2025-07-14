// frontend/src/components/WorkspaceNavbar.tsx
import React, { useState, useEffect, useRef } from 'react';
import { Link, useLocation } from 'react-router-dom';
import '../styles/Workspace/WorkspaceNavbar.css';

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  avatarUrl?: string;
  organizationId: string;
  organizationName?: string;
}

interface Organization {
  id: string;
  name: string;
  logoUrl?: string;
}

const NewWorkspaceNavbar: React.FC = () => {
  const location = useLocation();
  const [user, setUser] = useState<User | null>(null);
  const [userOrganizations, setUserOrganizations] = useState<Organization[]>([]);
  const [currentOrganization, setCurrentOrganization] = useState<Organization | null>(null);
  const [currentLanguage, setCurrentLanguage] = useState('FR');
  
  // Dropdown states
  const [orgDropdownOpen, setOrgDropdownOpen] = useState(false);
  const [pagesDropdownOpen, setPagesDropdownOpen] = useState(false);
  const [profileDropdownOpen, setProfileDropdownOpen] = useState(false);
  const [languageDropdownOpen, setLanguageDropdownOpen] = useState(false);

  // Refs for dropdown management
  const orgDropdownRef = useRef<HTMLDivElement>(null);
  const pagesDropdownRef = useRef<HTMLDivElement>(null);
  const profileDropdownRef = useRef<HTMLDivElement>(null);
  const languageDropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const userInfo = localStorage.getItem('currentUser');
    if (userInfo) {
      const userData = JSON.parse(userInfo);
      setUser(userData);
      
      // ✅ Fix: Use undefined instead of null for optional properties
      setCurrentOrganization({ 
        id: userData.organizationId, 
        name: userData.organizationName || 'SpaceLogic',
        logoUrl: undefined // Changed from null to undefined
      });
      
      // Fetch user organizations from API
      fetchUserOrganizations(userData.id);
    }
  }, []);

  const fetchUserOrganizations = async (userId: string) => {
    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/api/users/${userId}/organizations`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (response.ok) {
        const organizations = await response.json();
        setUserOrganizations(organizations);
      }
    } catch (error) {
      console.error('Error fetching user organizations:', error);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    window.location.href = '/login';
  };

  const switchOrganization = async (organization: Organization) => {
    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/api/users/switch-organization`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ organizationId: organization.id })
      });

      if (response.ok) {
        const responseData = await response.json();
        
        // Handle the new response format with nested user object
        const updatedUserData = responseData.user || responseData;
        
        // Update local storage with new token and user data
        if (responseData.token) {
          localStorage.setItem('authToken', responseData.token);
        }
        
        // ✅ Fix: Ensure all required User properties are present
        const mergedUserData: User = {
          id: updatedUserData.id || user?.id || '',
          firstName: updatedUserData.firstName || user?.firstName || '',
          lastName: updatedUserData.lastName || user?.lastName || '',
          email: updatedUserData.email || user?.email || '',
          avatarUrl: updatedUserData.avatarUrl || user?.avatarUrl,
          organizationId: responseData.organizationId || updatedUserData.organizationId || '',
          organizationName: responseData.organizationName || updatedUserData.organizationName
        };
        
        localStorage.setItem('currentUser', JSON.stringify(mergedUserData));
        
        // Update state
        setCurrentOrganization(organization);
        setUser(mergedUserData);
        setOrgDropdownOpen(false);
        
        // Redirect to refresh the workspace context
        window.location.href = '/workspace/myprojects';
      } else {
        const errorData = await response.json();
        console.error('Failed to switch organization:', errorData);
        alert(errorData.message || 'Failed to switch organization. Please try again.');
      }
    } catch (error) {
      console.error('Error switching organization:', error);
      alert('Error switching organization. Please try again.');
    }
  };

  const switchLanguage = (lang: string) => {
    setCurrentLanguage(lang);
    setLanguageDropdownOpen(false);
    // Here you would implement language switching logic
  };

  const workspacePages = [
    { path: '/workspace/myprojects', label: 'My projects', icon: 'folder' },
    { path: '/workspace/administration', label: 'Administration', icon: 'settings' },
    { path: '/workspace/users', label: 'Users and permissions', icon: 'users' },
    { path: '/workspace/activities', label: 'Activities', icon: 'activity' },
  ];

  // ✅ Fix: Explicitly type the return value and fix the default case
  const renderIcon = (iconName: string): React.ReactElement => {
    const iconProps = {
      width: "16",
      height: "16",
      viewBox: "0 0 24 24",
      fill: "none",
      stroke: "currentColor",
      strokeWidth: "2",
      strokeLinecap: "round" as const,
      strokeLinejoin: "round" as const
    };

    switch (iconName) {
      case 'folder':
        return (
          <svg {...iconProps}>
            <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path>
          </svg>
        );
      case 'settings':
        return (
          <svg {...iconProps}>
            <circle cx="12" cy="12" r="3"></circle>
            <path d="M12 1v6m0 6v6m11-7h-6m-6 0H1m17-4h-4m-6 0H1m17 8h-4m-6 0H1"></path>
          </svg>
        );
      case 'users':
        return (
          <svg {...iconProps}>
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
            <circle cx="9" cy="7" r="4"></circle>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87m-4-12a4 4 0 0 1 0 7.75"></path>
          </svg>
        );
      case 'activity':
        return (
          <svg {...iconProps}>
            <polyline points="22,12 18,12 15,21 9,3 6,12 2,12"></polyline>
          </svg>
        );
      case 'chevron-down':
        return (
          <svg {...iconProps}>
            <polyline points="6,9 12,15 18,9"></polyline>
          </svg>
        );
      case 'globe':
        return (
          <svg {...iconProps}>
            <circle cx="12" cy="12" r="10"></circle>
            <line x1="2" y1="12" x2="22" y2="12"></line>
            <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z"></path>
          </svg>
        );
      default:
        // ✅ Fix: Return a valid React element instead of null
        return (
          <svg {...iconProps}>
            <circle cx="12" cy="12" r="1"></circle>
          </svg>
        );
    }
  };

  const getUserInitials = () => {
    if (!user) return 'U';
    return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
  };

  return (
    <nav className="new-workspace-navbar">
      <div className="new-workspace-navbar-container">
        {/* Left side */}
        <div className="navbar-left">
          {/* Logo */}
          <div className="navbar-logo">
            <Link to="/workspace/myprojects">SpaceLogic</Link>
          </div>

          {/* Organization Dropdown */}
          <div className="dropdown-container" ref={orgDropdownRef}>
            <button 
              className="dropdown-trigger"
              onClick={() => setOrgDropdownOpen(!orgDropdownOpen)}
            >
              <span>{currentOrganization?.name || 'Select Organization'}</span>
              {renderIcon('chevron-down')}
            </button>
            {orgDropdownOpen && (
              <div className="dropdown-menu">
                {userOrganizations.map((org) => (
                  <div
                    key={org.id}
                    className={`dropdown-item ${currentOrganization?.id === org.id ? 'active' : ''}`}
                    onClick={() => switchOrganization(org)}
                  >
                    {org.name}
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Workspace Pages Dropdown */}
          <div className="dropdown-container" ref={pagesDropdownRef}>
            <button 
              className="dropdown-trigger"
              onClick={() => setPagesDropdownOpen(!pagesDropdownOpen)}
            >
              <span>Workspace</span>
              {renderIcon('chevron-down')}
            </button>
            {pagesDropdownOpen && (
              <div className="dropdown-menu">
                {workspacePages.map((page) => (
                  <Link
                    key={page.path}
                    to={page.path}
                    className={`dropdown-item ${location.pathname === page.path ? 'active' : ''}`}
                    onClick={() => setPagesDropdownOpen(false)}
                  >
                    {renderIcon(page.icon)}
                    <span>{page.label}</span>
                  </Link>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Right side */}
        <div className="navbar-right">
          {/* Support, Contact, What's new buttons */}
          <div className="navbar-buttons">
            <Link to="/support" className="navbar-button">Support</Link>
            <Link to="/contact" className="navbar-button">Contact</Link>
            <Link to="/whats-new" className="navbar-button">What's new</Link>
          </div>

          {/* Language Dropdown */}
          <div className="dropdown-container" ref={languageDropdownRef}>
            <button 
              className="language-dropdown-trigger"
              onClick={() => setLanguageDropdownOpen(!languageDropdownOpen)}
              title="Change language"
            >
              {renderIcon('globe')}
            </button>
            {languageDropdownOpen && (
              <div className="dropdown-menu language-dropdown">
                <div
                  className={`dropdown-item ${currentLanguage === 'FR' ? 'active' : ''}`}
                  onClick={() => switchLanguage('FR')}
                >
                  Français
                </div>
                <div
                  className={`dropdown-item ${currentLanguage === 'EN' ? 'active' : ''}`}
                  onClick={() => switchLanguage('EN')}
                >
                  English
                </div>
              </div>
            )}
          </div>

          {/* Vertical separator */}
          <div className="vertical-separator"></div>

          {/* User Profile Dropdown */}
          <div className="dropdown-container" ref={profileDropdownRef}>
            <button 
              className="profile-dropdown-trigger"
              onClick={() => setProfileDropdownOpen(!profileDropdownOpen)}
            >
              <div className="user-avatar">
                {user?.avatarUrl ? (
                  <img src={user.avatarUrl} alt={`${user.firstName} ${user.lastName}`} />
                ) : (
                  <span className="user-initials">{getUserInitials()}</span>
                )}
              </div>
              <span className="user-name">{user ? `${user.firstName} ${user.lastName}` : 'User'}</span>
            </button>
            {profileDropdownOpen && (
              <div className="dropdown-menu profile-dropdown">
                <Link
                  to="/profile"
                  className="dropdown-item"
                  onClick={() => setProfileDropdownOpen(false)}
                >
                  My profile
                </Link>
                <div
                  className="dropdown-item"
                  onClick={handleLogout}
                >
                  Sign out
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};

export default NewWorkspaceNavbar;