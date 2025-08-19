// frontend/src/pages/workspace/UsersPage.tsx
import React, { useState } from 'react';
import WorkspaceNavbar from '../../components/WorkspaceNavbar';
import '../../styles/Workspace/Workspace.css';

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  department: string;
  lastActive: string;
  status: 'active' | 'inactive' | 'pending';
}

const UsersPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedRole, setSelectedRole] = useState('all');

  // Mock data - replace with actual API call
  const users: User[] = [
    {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@company.com',
      role: 'Admin',
      department: 'IT',
      lastActive: '2025-01-15',
      status: 'active'
    },
    {
      id: '2',
      firstName: 'Jane',
      lastName: 'Smith',
      email: 'jane.smith@company.com',
      role: 'User',
      department: 'Marketing',
      lastActive: '2025-01-14',
      status: 'active'
    }
  ];

  const getStatusBadge = (status: string) => {
    const classes = {
      active: 'status-badge status-active',
      inactive: 'status-badge status-inactive',
      pending: 'status-badge status-pending'
    };
    
    return <span className={classes[status as keyof typeof classes]}>{status}</span>;
  };

  return (
    <>
      <WorkspaceNavbar />
      <div className="updated-workspace-content">
        <div className="page-header">
          <h1 className="page-title">Users and permissions</h1>
          <p className="page-subtitle">Manage team members, roles, and access permissions</p>
        </div>

        <div className="workspace-card">
          <div className="projects-header">
            <h2>Team Members</h2>
            <div className="projects-actions">
              <div className="search-container">
                <svg className="search-icon" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.35-4.35"></path>
                </svg>
                <input
                  type="text"
                  placeholder="Search users..."
                  className="search-input"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
              <select 
                className="form-control"
                value={selectedRole}
                onChange={(e) => setSelectedRole(e.target.value)}
              >
                <option value="all">All Roles</option>
                <option value="admin">Admin</option>
                <option value="user">User</option>
                <option value="viewer">Viewer</option>
              </select>
              <button className="btn-primary">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <line x1="12" y1="5" x2="12" y2="19"></line>
                  <line x1="5" y1="12" x2="19" y2="12"></line>
                </svg>
                Invite User
              </button>
            </div>
          </div>

          <table className="workspace-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Department</th>
                <th>Last Active</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td>{user.firstName} {user.lastName}</td>
                  <td>{user.email}</td>
                  <td>{user.role}</td>
                  <td>{user.department}</td>
                  <td>{user.lastActive}</td>
                  <td>{getStatusBadge(user.status)}</td>
                  <td>
                    <button className="btn-secondary" style={{ marginRight: '8px' }}>Edit</button>
                    <button className="btn-secondary">Remove</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div className="workspace-card">
          <h2>Role Management</h2>
          <p>Define and manage user roles and their permissions across the organization.</p>
          <button className="btn-primary">Manage Roles</button>
        </div>
      </div>
    </>
  );
};

export default UsersPage;