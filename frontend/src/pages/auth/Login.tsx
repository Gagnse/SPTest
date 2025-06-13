// frontend/src/pages/auth/Login.tsx
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import axios from 'axios';
import '../../styles/login.css';

const Login: React.FC = () => {
  const { t } = useTranslation('auth/login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setMessage(null);

    try {
      // Call the login API endpoint
      const response = await axios.post(`${import.meta.env.VITE_API_URL || 'http://localhost:5000'}/api/auth/login`, {
        email,
        password,
      });

      if (response.data.success) {
        // Store the authentication token if provided
        if (response.data.token) {
          localStorage.setItem('authToken', response.data.token);
        }
        
        // Store user info if provided
        if (response.data.user) {
          localStorage.setItem('currentUser', JSON.stringify(response.data.user));
        }

        setMessage({ type: 'success', text: t('successMessage') });
        
        // Redirect to projects page after a short delay
        setTimeout(() => {
          window.location.href = '/workspace/projects';
        }, 1000);
      } else {
        setMessage({ type: 'error', text: response.data.message || t('errorMessage') });
      }
    } catch (error: any) {
      console.error('Login error:', error);
      
      if (error.response?.status === 401) {
        setMessage({ type: 'error', text: t('errorMessage') });
      } else if (error.response?.data?.message) {
        setMessage({ type: 'error', text: error.response.data.message });
      } else {
        setMessage({ type: 'error', text: 'Une erreur est survenue. Veuillez réessayer.' });
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="auth-container page-content">
      <div className="auth-form-container">
        <h1>{t('title')}</h1>

        {message && (
          <div className={`alert alert-${message.type}`}>{message.text}</div>
        )}

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="form-group">
            <label htmlFor="email">{t('email')}</label>
            <input
              className="form-control"
              type="email"
              id="email"
              name="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={isLoading}
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">{t('password')}</label>
            <input
              className="form-control"
              type="password"
              id="password"
              name="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={isLoading}
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={isLoading}>
              {isLoading ? 'Connexion...' : t('submit')}
            </button>
          </div>

          <div className="auth-links">
            <p>
              {t('signupPrompt')}{' '}
              <a href="/signup">{t('signupLink')}</a>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Login;