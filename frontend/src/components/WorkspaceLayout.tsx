// frontend/src/components/UpdatedWorkspaceLayout.tsx
import React from 'react';
import WorkspaceNavbar from './WorkspaceNavbar';
import '../styles/Workspace/Workspace.css';

interface UpdatedWorkspaceLayoutProps {
  children: React.ReactNode;
}

const UpdatedWorkspaceLayout: React.FC<UpdatedWorkspaceLayoutProps> = ({ children }) => {
  return (
    <div className="updated-workspace-layout">
      {/* New Workspace Navbar - no secondary navbar */}
      <WorkspaceNavbar />
      
      {/* Workspace Content Area */}
      <div className="updated-workspace-content">
        {children}
      </div>
    </div>
  );
};

export default UpdatedWorkspaceLayout;