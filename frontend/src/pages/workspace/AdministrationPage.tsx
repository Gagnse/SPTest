// frontend/src/pages/workspace/AdministrationPage.tsx
import React from 'react';
import WorkspaceNavbar from '../../components/WorkspaceNavbar';
import '../../styles/Workspace/workspace.css';

const AdministrationPage: React.FC = () => {
  return (
    <>
      <WorkspaceNavbar />
      <div className="updated-workspace-content">
        <div className="page-header">
          <h1 className="page-title">Administration</h1>
          <p className="page-subtitle">Manage organization settings and configuration</p>
        </div>

        <div className="workspace-card">
          <h2>Organization Settings</h2>
          <p>Configure your organization's general settings, branding, and preferences.</p>
          <button className="btn-primary">Edit Settings</button>
        </div>

        <div className="workspace-card">
          <h2>Security & Compliance</h2>
          <p>Manage security policies, compliance settings, and audit logs.</p>
          <button className="btn-primary">Configure Security</button>
        </div>

        <div className="workspace-card">
          <h2>Integrations</h2>
          <p>Connect third-party services and manage API keys.</p>
          <button className="btn-primary">Manage Integrations</button>
        </div>

        <div className="workspace-card">
          <h2>Billing & Subscription</h2>
          <p>View billing information and manage your subscription plan.</p>
          <button className="btn-primary">View Billing</button>
        </div>
      </div>
    </>
  );
};

export default AdministrationPage;