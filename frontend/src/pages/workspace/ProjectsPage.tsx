// frontend/src/pages/workspace/ProjectsPage.tsx
import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import WorkspaceNavbar from '../../components/WorkspaceNavbar';
import axios from 'axios';
import '../../styles/Workspace/workspace.css';

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

const ProjectsPage: React.FC = () => {
  const { t } = useTranslation('common');
  const [activeTab, setActiveTab] = useState<'active' | 'archive'>('active');
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [projects, setProjects] = useState<Project[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);


  // Form state for new project
  const [newProject, setNewProject] = useState({
    projectNumber: '',
    name: '',
    description: '',
    type: ''
  });

  // Load projects on component mount
  useEffect(() => {
    // Check if user is authenticated
    const token = localStorage.getItem('authToken');
    
    if (!token) {
      window.location.href = '/login';
      return;
    }

    fetchProjects();
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



  // Filter projects based on active tab and search term
  const filteredProjects = projects.filter(project => {
    const matchesTab = activeTab === 'active' 
      ? project.status !== 'archive' 
      : project.status === 'archive';
    
    const matchesSearch = project.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         project.projectNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         project.description.toLowerCase().includes(searchTerm.toLowerCase());
    
    return matchesTab && matchesSearch;
  });

  const getStatusBadge = (status: string) => {
    const statusClasses = {
      active: 'status-badge status-active',
      pending: 'status-badge status-pending',
      completed: 'status-badge status-completed',
      archive: 'status-badge status-archive'
    };

    const statusTexts = {
      active: 'Actif',
      pending: 'En attente',
      completed: 'Terminé',
      archive: 'Archivé'
    };

    return (
      <span className={statusClasses[status as keyof typeof statusClasses]}>
        {statusTexts[status as keyof typeof statusTexts]}
      </span>
    );
  };

  if (isLoading) {
    return (
      <>
        <WorkspaceNavbar />
        <div className="workspace-content">
          <div className="loading-container">
            <div className="loading-spinner"></div>
            <p>{t('workspace.loadingProjects')}</p>
          </div>
        </div>
      </>
    );
  }

  if (error) {
    return (
      <>
        <WorkspaceNavbar />
        <div className="workspace-content">
          <div className="error-container">
            <div className="error-icon">⚠️</div>
            <h2>Erreur</h2>
            <p>{error}</p>
            <button className="btn btn-primary" onClick={fetchProjects}>
              Réessayer
            </button>
          </div>
        </div>
      </>
    );
  }

  return (
    <>
      <WorkspaceNavbar />
      <div className="workspace-content">
        <div className="projects-container">
          <div className="projects-header">
            <h1>Mes projets</h1>
            <div className="projects-actions">
              <div className="search-container">
                <svg className="search-icon" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="11" cy="11" r="8"></circle>
                  <path d="m21 21-4.35-4.35"></path>
                </svg>
                <input
                  type="text"
                  placeholder="Rechercher un projet..."
                  className="search-input"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
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
          </div>

          {/* Tab Navigation */}
          <div className="projects-tabs">
            <button 
              className={`tab-btn ${activeTab === 'active' ? 'active' : ''}`}
              onClick={() => setActiveTab('active')}
            >
              Projets actifs ({projects.filter(p => p.status !== 'archive').length})
            </button>
            <button 
              className={`tab-btn ${activeTab === 'archive' ? 'active' : ''}`}
              onClick={() => setActiveTab('archive')}
            >
              Projets archivés ({projects.filter(p => p.status === 'archive').length})
            </button>
          </div>

          {/* Projects Grid */}
          <div className="projects-grid">
            {filteredProjects.length === 0 ? (
              <div className="empty-state">
                <div className="empty-state-icon">📁</div>
                <h3>Aucun projet trouvé</h3>
                <p>
                  {searchTerm 
                    ? "Aucun projet ne correspond à votre recherche." 
                    : activeTab === 'active' 
                      ? "Vous n'avez pas encore de projets actifs." 
                      : "Vous n'avez pas de projets archivés."
                  }
                </p>
                {!searchTerm && activeTab === 'active' && (
                  <button 
                    className="btn btn-primary"
                    onClick={() => setIsModalOpen(true)}
                  >
                    Créer votre premier projet
                  </button>
                )}
              </div>
            ) : (
              filteredProjects.map((project) => (
                <div key={project.id} className="project-card">
                  <div className="project-image">
                    {project.imageUrl ? (
                      <img src={project.imageUrl} alt={project.name} />
                    ) : (
                      <div className="project-placeholder">
                        <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                          <path d="M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z"></path>
                        </svg>
                      </div>
                    )}
                  </div>
                  <div className="project-content">
                    <div className="project-header">
                      <h3 className="project-title">{project.name}</h3>
                      {getStatusBadge(project.status)}
                    </div>
                    <p className="project-number">#{project.projectNumber}</p>
                    <p className="project-description">{project.description}</p>
                    {project.organizationName && (
                      <p className="project-organization">{project.organizationName}</p>
                    )}
                    <div className="project-dates">
                      <span className="project-start-date">
                        Début: {new Date(project.startDate).toLocaleDateString('fr-FR')}
                      </span>
                      {project.endDate && (
                        <span className="project-end-date">
                          Fin: {new Date(project.endDate).toLocaleDateString('fr-FR')}
                        </span>
                      )}
                    </div>
                  </div>
                  <div className="project-actions">
                    <button className="btn btn-secondary btn-sm">
                      Voir détails
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      {/* Create Project Modal */}
      {isModalOpen && (
        <div className="modal-overlay" onClick={() => setIsModalOpen(false)}>
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
            <form onSubmit={handleCreateProject} className="modal-body">
              <div className="form-group">
                <label htmlFor="projectNumber">Numéro de projet *</label>
                <input
                  type="text"
                  id="projectNumber"
                  value={newProject.projectNumber}
                  onChange={(e) => setNewProject({...newProject, projectNumber: e.target.value})}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="name">Nom du projet *</label>
                <input
                  type="text"
                  id="name"
                  value={newProject.name}
                  onChange={(e) => setNewProject({...newProject, name: e.target.value})}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="description">Description</label>
                <textarea
                  id="description"
                  rows={3}
                  value={newProject.description}
                  onChange={(e) => setNewProject({...newProject, description: e.target.value})}
                />
              </div>
              <div className="form-group">
                <label htmlFor="type">Type de projet</label>
                <select
                  id="type"
                  value={newProject.type}
                  onChange={(e) => setNewProject({...newProject, type: e.target.value})}
                >
                  <option value="">Sélectionner un type</option>
                  <option value="interior">Aménagement intérieur</option>
                  <option value="construction">Construction</option>
                  <option value="renovation">Rénovation</option>
                  <option value="commercial">Commercial</option>
                  <option value="residential">Résidentiel</option>
                </select>
              </div>
              <div className="form-actions">
                <button 
                  type="button" 
                  className="btn btn-secondary"
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
      )}
    </>
  );
};

export default ProjectsPage;