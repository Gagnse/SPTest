import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import '../../styles/login.css';

const Login: React.FC = () => {
  const navigate = useNavigate();
  const { t } = useTranslation('auth/login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (email === 'test@example.com' && password === '1234') {
      setMessage({ type: 'success', text: t('successMessage') });
      setTimeout(() => navigate('/projets'), 1000);
    } else {
      setMessage({ type: 'error', text: t('errorMessage') });
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
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {t('submit')}
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
