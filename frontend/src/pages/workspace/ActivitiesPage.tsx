// frontend/src/pages/workspace/ActivitiesPage.tsx
import React, { useState } from 'react';
import WorkspaceNavbar from '../../components/WorkspaceNavbar';
import '../../styles/Workspace/workspace.css';

interface Activity {
  id: string;
  user: string;
  action: string;
  target: string;
  timestamp: string;
  type: 'project' | 'user' | 'organization' | 'system';
}

const ActivitiesPage: React.FC = () => {
  const [selectedFilter, setSelectedFilter] = useState('all');
  const [dateRange, setDateRange] = useState('7days');

  // Mock data - replace with actual API call
  const activities: Activity[] = [
    {
      id: '1',
      user: 'John Doe',
      action: 'created',
      target: 'Project Alpha',
      timestamp: '2025-01-16 10:30 AM',
      type: 'project'
    },
    {
      id: '2',
      user: 'Jane Smith',
      action: 'invited',
      target: 'mike@company.com',
      timestamp: '2025-01-16 09:15 AM',
      type: 'user'
    },
    {
      id: '3',
      user: 'Admin',
      action: 'updated',
      target: 'Organization Settings',
      timestamp: '2025-01-15 04:20 PM',
      type: 'organization'
    },
    {
      id: '4',
      user: 'System',
      action: 'backup completed',
      target: 'Database',
      timestamp: '2025-01-15 02:00 AM',
      type: 'system'
    }
  ];

  const getActivityIcon = (type: string) => {
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

    switch (type) {
      case 'project':
        return (
          <svg {...iconProps} style={{ color: '#3498db' }}>
            <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path>
          </svg>
        );
      case 'user':
        return (
          <svg {...iconProps} style={{ color: '#27ae60' }}>
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
            <circle cx="9" cy="7" r="4"></circle>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87m-4-12a4 4 0 0 1 0 7.75"></path>
          </svg>
        );
      case 'organization':
        return (
          <svg {...iconProps} style={{ color: '#e67e22' }}>
            <circle cx="12" cy="12" r="3"></circle>
            <path d="M12 1v6m0 6v6m11-7h-6m-6 0H1m17-4h-4m-6 0H1m17 8h-4m-6 0H1"></path>
          </svg>
        );
      case 'system':
        return (
          <svg {...iconProps} style={{ color: '#9b59b6' }}>
            <rect x="2" y="3" width="20" height="14" rx="2" ry="2"></rect>
            <line x1="8" y1="21" x2="16" y2="21"></line>
            <line x1="12" y1="17" x2="12" y2="21"></line>
          </svg>
        );
      default:
        return (
          <svg {...iconProps}>
            <circle cx="12" cy="12" r="1"></circle>
          </svg>
        );
    }
  };

  const filteredActivities = activities.filter(activity => 
    selectedFilter === 'all' || activity.type === selectedFilter
  );

  return (
    <>
      <WorkspaceNavbar />
      <div className="updated-workspace-content">
        <div className="page-header">
          <h1 className="page-title">Activities</h1>
          <p className="page-subtitle">Track and monitor all activities within your organization</p>
        </div>

        <div className="workspace-card">
          <div className="projects-header">
            <h2>Activity Log</h2>
            <div className="projects-actions">
              <select 
                className="form-control"
                value={selectedFilter}
                onChange={(e) => setSelectedFilter(e.target.value)}
              >
                <option value="all">All Activities</option>
                <option value="project">Projects</option>
                <option value="user">Users</option>
                <option value="organization">Organization</option>
                <option value="system">System</option>
              </select>
              <select 
                className="form-control"
                value={dateRange}
                onChange={(e) => setDateRange(e.target.value)}
              >
                <option value="7days">Last 7 days</option>
                <option value="30days">Last 30 days</option>
                <option value="90days">Last 90 days</option>
                <option value="custom">Custom range</option>
              </select>
              <button className="btn-primary">Export Log</button>
            </div>
          </div>

          <div style={{ background: 'white', borderRadius: '8px', overflow: 'hidden' }}>
            {filteredActivities.map((activity) => (
              <div 
                key={activity.id} 
                style={{ 
                  display: 'flex', 
                  alignItems: 'center', 
                  padding: '16px', 
                  borderBottom: '1px solid #f7fafc',
                  gap: '12px'
                }}
              >
                {getActivityIcon(activity.type)}
                <div style={{ flex: 1 }}>
                  <div style={{ fontWeight: '500', color: '#2d3748' }}>
                    <strong>{activity.user}</strong> {activity.action} <strong>{activity.target}</strong>
                  </div>
                  <div style={{ fontSize: '0.875rem', color: '#718096', marginTop: '4px' }}>
                    {activity.timestamp}
                  </div>
                </div>
                <span 
                  style={{ 
                    padding: '4px 8px', 
                    borderRadius: '4px', 
                    fontSize: '0.75rem', 
                    fontWeight: '500',
                    backgroundColor: '#f7fafc',
                    color: '#4a5568',
                    textTransform: 'capitalize'
                  }}
                >
                  {activity.type}
                </span>
              </div>
            ))}
          </div>
        </div>

        <div className="workspace-card">
          <h2>Activity Analytics</h2>
          <p>View detailed analytics and insights about organization activity patterns.</p>
          <button className="btn-primary">View Analytics</button>
        </div>
      </div>
    </>
  );
};

export default ActivitiesPage;