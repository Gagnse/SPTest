// frontend/src/pages/auth/Login.tsx
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import '../../styles/login.css';

const Login: React.FC = () => {
  const { t } = useTranslation('auth/login');
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Handle login logic here
    console.log('Login attempt:', formData);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  return (
    <div className="page-content">
      <div className="container">
        <div className="login-container">
          <form className="login-form" onSubmit={handleSubmit}>
            <h1>{t('title')}</h1>
            
            <div className="form-group">
              <label htmlFor="email">{t('email')}</label>
              <input
                type="email"
                id="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
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
              />
            </div>
            
            <button type="submit" className="login-button">
              {t('submit')}
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