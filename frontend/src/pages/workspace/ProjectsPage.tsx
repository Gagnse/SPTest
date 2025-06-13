// frontend/src/pages/workspace/ProjectsPage.tsx
import React, { useState, useEffect } from 'react';
import WorkspaceLayout from '../../components/WorkspaceLayout';
import axios from 'axios';
import '../../styles/workspace.css';

interface Project {
  id: string;
  projectNumber: string;
  name: string;
  description: string;
  status: 'active' | 'pending' | 'completed' | 'archive';
  type?: string;
  imageUrl?: string;
  startDate: string;
  endDate?: string;
  organizationName?: string;
}

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role?: string;
  organizationName?: string;
}

const ProjectsPage: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'actif' | 'archive'>('actif');
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [projects, setProjects] = useState<Project[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(null);

  // Form state for new project
  const [newProject, setNewProject] = useState({
    projectNumber: '',
    name: '',
    description: '',
    type: ''
  });

  // Load user info and projects on component mount
  useEffect(() => {
    // Check if user is authenticated
    const token = localStorage.getItem('authToken');
    const userInfo = localStorage.getItem('currentUser');
    
    if (!token) {
      window.location.href = '/login';
      return;
    }

    if (userInfo) {
      setUser(JSON.parse(userInfo));
    }

    fetchProjects();
    
    // Load saved tab preference
    const savedTab = localStorage.getItem('activeProjectTab') as 'actif' | 'archive';
    if (savedTab) {
      setActiveTab(savedTab);
    }
  }, []);

  const fetchProjects = async () => {
    try {
      setIsLoading(true);
      const token = localStorage.getItem('authToken');
      
      const response = await axios.get(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/api/projects`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      setProjects(response.data);
      setError(null);
    } catch (error: any) {
      console.error('Error fetching projects:', error);
      
      if (error.response?.status === 401) {
        // Token expired or invalid
        localStorage.removeItem('authToken');
        localStorage.removeItem('currentUser');
        window.location.href = '/login';
      } else {
        setError('Erreur lors du chargement des projets.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleCreateProject = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      const token = localStorage.getItem('authToken');
      
      const response = await axios.post(
        `${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/api/projects`,
        newProject,
        {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
          }
        }
      );

      // Add the new project to the list
      setProjects([...projects, response.data]);
      
      // Reset form and close modal
      setNewProject({
        projectNumber: '',
        name: '',
        description: '',
        type: ''
      });
      setIsModalOpen(false);
      
    } catch (error: any) {
      console.error('Error creating project:', error);
      if (error.response?.status === 401) {
        localStorage.removeItem('authToken');
        localStorage.removeItem('currentUser');
        window.location.href = '/login';
      } else {
        alert(error.response?.data?.message || 'Erreur lors de la création du projet.');
      }
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('currentUser');
    window.location.href = '/login';
  };

  // Filter projects based on active tab
  const filteredProjects = projects.filter(project => {
    if (activeTab === 'actif') {
      return project.status !== 'archive';
    } else {
      return project.status === 'archive';
    }
  });

  // Further filter by search term
  const searchFilteredProjects = filteredProjects.filter(project => {
    const searchLower = searchTerm.toLowerCase();
    return (
      project.name.toLowerCase().includes(searchLower) ||
      project.projectNumber.toLowerCase().includes(searchLower) ||
      (project.description && project.description.toLowerCase().includes(searchLower))
    );
  });

  const handleTabChange = (tab: 'actif' | 'archive') => {
    setActiveTab(tab);
    setSearchTerm(''); // Clear search on tab change
    localStorage.setItem('activeProjectTab', tab);
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case 'active': return 'Actif';
      case 'pending': return 'Pause';
      case 'completed': return 'Terminé';
      case 'archive': return 'Archivé';
      default: return status;
    }
  };

  const getStatusClass = (status: string) => {
    switch (status) {
      case 'active': return 'active';
      case 'pending': return 'pending';
      case 'completed': return 'completed';
      case 'archive': return 'archive';
      default: return status.toLowerCase();
    }
  };

  const workspaceContext = (
    <div className="workspace-actions">
      <div className="user-info">
        {user && (
          <span className="welcome-text">
            Bonjour, {user.firstName} {user.lastName}
          </span>
        )}
        <button 
          className="btn btn-secondary logout-btn"
          onClick={handleLogout}
          title="Se déconnecter"
        >
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
            <polyline points="16 17 21 12 16 7"></polyline>
            <line x1="21" y1="12" x2="9" y2="12"></line>
          </svg>
        </button>
      </div>
      <button 
        className="btn btn-primary create-project-btn"
        onClick={() => setIsModalOpen(true)}
      >
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
          <line x1="12" y1="5" x2="12" y2="19"></line>
          <line x1="5" y1="12" x2="19" y2="12"></line>
        </svg>
        Nouveau projet
      </button>
    </div>
  );

  if (isLoading) {
    return (
      <WorkspaceLayout workspaceContext={workspaceContext} currentPath="/workspace/projects">
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p>Chargement des projets...</p>
        </div>
      </WorkspaceLayout>
    );
  }

  if (error) {
    return (
      <WorkspaceLayout workspaceContext={workspaceContext} currentPath="/workspace/projects">
        <div className="error-container">
          <div className="error-icon">⚠️</div>
          <h2>Erreur</h2>
          <p>{error}</p>
          <button className="btn btn-primary" onClick={fetchProjects}>
            Réessayer
          </button>
        </div>
      </WorkspaceLayout>
    );
  }

  return (
    <>
      <WorkspaceLayout workspaceContext={workspaceContext} currentPath="/workspace/projects">
        <div className="projects-container">
          <div className="projects-header">
            <h1>Mes projets</h1>
            <div className="projects-filters">
              <div className="search-container">
                <input 
                  type="text" 
                  placeholder="Rechercher un projet..." 
                  className="search-input"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="search-icon">
                  <circle cx="11" cy="11" r="8"></circle>
                  <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
                </svg>
              </div>
              <div className="filter-dropdown">
                <button className="filter-btn">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon>
                  </svg>
                  Filtrer
                </button>
              </div>
              <div className="sort-dropdown">
                <button className="sort-btn">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <line x1="12" y1="5" x2="12" y2="19"></line>
                    <polyline points="19 12 12 19 5 12"></polyline>
                  </svg>
                  Trier
                </button>
              </div>
            </div>
          </div>

          <div className="tabs-container">
            <div className="tabs">
              <button 
                className={`tab-btn ${activeTab === 'actif' ? 'active' : ''}`}
                onClick={() => handleTabChange('actif')}
              >
                Actif ({filteredProjects.filter(p => p.status !== 'archive').length})
              </button>
              <button 
                className={`tab-btn ${activeTab === 'archive' ? 'active' : ''}`}
                onClick={() => handleTabChange('archive')}
              >
                Archivé ({filteredProjects.filter(p => p.status === 'archive').length})
              </button>
            </div>

            <div className="tab-content active">
              {searchFilteredProjects.length > 0 ? (
                <div className="projects-grid">
                  {searchFilteredProjects.map((project) => (
                    <a href={`/workspace/project/${project.id}`} key={project.id} className="project-card-link">
                      <div className="project-card">
                        <div className="project-card-header">
                          <p className="project-number">No. {project.projectNumber}</p>
                          <div className="project-actions">
                            <button className="project-action-btn" onClick={(e) => {
                              e.preventDefault();
                              e.stopPropagation();
                              // Handle project actions menu
                            }}>
                              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <circle cx="12" cy="12" r="1"></circle>
                                <circle cx="19" cy="12" r="1"></circle>
                                <circle cx="5" cy="12" r="1"></circle>
                              </svg>
                            </button>
                          </div>
                        </div>
                        <div className="project-card-content">
                          <div className="project-image-container">
                            <img 
                              src={project.imageUrl || "/static/images/project-placeholder.png"} 
                              alt="Project Image" 
                              className="project-image"
                            />
                          </div>
                          <h3 className="project-title">{project.name}</h3>
                          <p className="project-description">{project.description || 'Aucune description'}</p>
                        </div>
                        <div className="project-card-footer">
                          <div className="project-name">
                            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                              <path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"></path>
                              <path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"></path>
                            </svg>
                            <span>{project.projectNumber}</span>
                          </div>
                          <span className={`project-status ${getStatusClass(project.status)}`}>
                            {getStatusLabel(project.status)}
                          </span>
                        </div>
                      </div>
                    </a>
                  ))}
                </div>
              ) : (
                <div className="empty-state">
                  <div className="empty-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path>
                    </svg>
                  </div>
                  <h2>
                    {activeTab === 'actif' ? 'Aucun projet actif' : 'Aucun projet archivé'}
                  </h2>
                  <p>
                    {activeTab === 'actif' 
                      ? searchTerm 
                        ? 'Aucun projet ne correspond à votre recherche.'
                        : "Vous n'avez pas encore créé ou rejoint de projet actif."
                      : "Vous n'avez pas de projets archivés."
                    }
                  </p>
                </div>
              )}
            </div>
          </div>
        </div>
      </WorkspaceLayout>

      {/* Project Creation Modal */}
      {isModalOpen && (
        <div className="modal active" onClick={() => setIsModalOpen(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Créer un nouveau projet</h2>
              <button 
                className="close-modal-btn"
                onClick={() => setIsModalOpen(false)}
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <line x1="18" y1="6" x2="6" y2="18"></line>
                  <line x1="6" y1="6" x2="18" y2="18"></line>
                </svg>
              </button>
            </div>
            <div className="modal-body">
              <form onSubmit={handleCreateProject}>
                <div className="form-group">
                  <label htmlFor="project-name">Nom du projet *</label>
                  <input 
                    type="text" 
                    id="project-name" 
                    name="project-name" 
                    required 
                    value={newProject.name}
                    onChange={(e) => setNewProject({...newProject, name: e.target.value})}
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="project-number">Numéro du projet *</label>
                  <input 
                    type="text" 
                    id="project-number" 
                    name="project-number" 
                    required 
                    value={newProject.projectNumber}
                    onChange={(e) => setNewProject({...newProject, projectNumber: e.target.value})}
                    placeholder="Ex: P2025-001"
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="project-description">Description</label>
                  <textarea 
                    id="project-description" 
                    name="project-description" 
                    rows={4}
                    value={newProject.description}
                    onChange={(e) => setNewProject({...newProject, description: e.target.value})}
                  ></textarea>
                </div>
                <div className="form-group">
                  <label htmlFor="project-type">Type de projet</label>
                  <select 
                    id="project-type" 
                    name="project-type"
                    value={newProject.type}
                    onChange={(e) => setNewProject({...newProject, type: e.target.value})}
                  >
                    <option value="">Sélectionner un type</option>
                    <option value="residential">Résidentiel</option>
                    <option value="commercial">Commercial</option>
                    <option value="industrial">Industriel</option>
                    <option value="institutional">Institutionnel</option>
                  </select>
                </div>
                <div className="form-actions">
                  <button 
                    type="button" 
                    className="btn btn-secondary cancel-btn"
                    onClick={() => setIsModalOpen(false)}
                  >
                    Annuler
                  </button>
                  <button type="submit" className="btn btn-primary">
                    Créer le projet
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default ProjectsPage;