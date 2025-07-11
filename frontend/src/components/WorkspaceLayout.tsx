// frontend/src/components/WorkspaceLayout.tsx
import React from 'react';
import '../styles/Workspace/workspace.css'; // Import the CSS file

interface WorkspaceLayoutProps {
  children: React.ReactNode;
  workspaceContext?: React.ReactNode;
  currentPath?: string;
}

const WorkspaceLayout: React.FC<WorkspaceLayoutProps> = ({ 
  children, 
  workspaceContext, 
  currentPath = '/workspace/myprojects' 
}) => {
  const isActive = (path: string) => currentPath === path;

  return (
    <div className="workspace-layout">
      {/* Workspace Secondary Navbar */}
      <div className="workspace-navbar">
        <div className="workspace-navbar-container">
          <div className="workspace-nav-items">
            <a 
              href="/workspace/myprojects" 
              className={`workspace-nav-item ${isActive('/workspace/myprojects') ? 'active' : ''}`}
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path>
              </svg>
              <span>Mes projets</span>
            </a>
            <a 
              href="/workspace/organisations" 
              className={`workspace-nav-item ${isActive('/workspace/organisations') ? 'active' : ''}`}
            >
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"></path>
                <circle cx="12" cy="8" r="2"></circle>
                <path d="M12 10c-2.21 0-4 1.79-4 4v2h8v-2c0-2.21-1.79-4-4-4z"></path>
              </svg>
              <span>Organisations</span>
            </a>
          </div>
          <div className="workspace-context">
            {workspaceContext}
          </div>
        </div>
      </div>

      {/* Workspace Content Area */}
      <div className="workspace-content">
        {children}
      </div>
    </div>
  );
};

export default WorkspaceLayout;