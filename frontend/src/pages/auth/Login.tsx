// frontend/src/pages/auth/Login.tsx
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import '../../styles/login.css';

interface LoginResponse {
  success: boolean;
  data?: {
    token: string;
    user: {
      id: string;
      firstName: string;
      lastName: string;
      email: string;
      organizationId: string;
      organizationName?: string;
      role?: string;
      department?: string;
      location?: string;
      avatarUrl?: string;
    };
  };
  message?: string;
}

const Login: React.FC = () => {
  const { t } = useTranslation('auth/login');
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string>('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Reset error state
    setError('');
    setIsLoading(true);

    try {
      const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000';
      
      const response = await fetch(`${apiUrl}/api/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: formData.email,
          password: formData.password,
        }),
      });

      const result: LoginResponse = await response.json();

      if (response.ok && result.success && result.data) {
        // Store authentication token
        localStorage.setItem('authToken', result.data.token);
        
        // Store user information
        localStorage.setItem('currentUser', JSON.stringify(result.data.user));

        // Small delay as requested
        setTimeout(() => {
          // Redirect to workspace
          window.location.href = '/workspace/myprojects';
        }, 800); // 800ms delay for smooth transition

      } else {
        // Handle authentication error
        setError(result.message || t('errorMessage'));
      }
    } catch (error) {
      console.error('Login error:', error);
      setError('Network error. Please check your connection and try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
    // Clear error when user starts typing
    if (error) setError('');
  };

  return (
    <div className="page-content">
      <div className="container">
        <div className="login-container">
          <form className="login-form" onSubmit={handleSubmit}>
            <h1>{t('title')}</h1>
            
            {error && (
              <div className="error-message">
                {error}
              </div>
            )}
            
            <div className="form-group">
              <label htmlFor="email">{t('email')}</label>
              <input
                type="email"
                id="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
                disabled={isLoading}
              />
            </div>
            
            <div className="form-group">
              <label htmlFor="password">{t('password')}</label>
              <input
                type="password"
                id="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
                disabled={isLoading}
              />
            </div>
            
            <button type="submit" className="login-button" disabled={isLoading}>
              {isLoading ? 'Signing in...' : t('submit')}
            </button>
            
            <div className="forgot-password">
              <Link to="/forgot-password">{t('forgotPassword')}</Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Login;