import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import LanguageSwitcher from './LanguageSwitcher';

const Navbar: React.FC = () => {
  const location = useLocation();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const { t } = useTranslation('common'); // 👈 traduction depuis common.json

  const isActive = (path: string) => location.pathname === path;

  return (
    <header>
      <nav className="navbar">
        <div className="navbar-container">
          <div className="navbar-left">
            <div className="navbar-logo">
              <Link to="/">SpaceLogic</Link>
            </div>
            <ul className={`navbar-menu ${isMobileMenuOpen ? 'active' : ''}`}>
              <li className="navbar-item">
                <Link to="/" className={`navbar-link ${isActive('/') ? 'active' : ''}`}>{t('navbar.home')}</Link>
              </li>
              <li className="navbar-item">
                <Link to="/about" className={`navbar-link ${isActive('/about') ? 'active' : ''}`}>{t('navbar.about')}</Link>
              </li>
              <li className="navbar-item">
                <Link to="/services" className={`navbar-link ${isActive('/services') ? 'active' : ''}`}>{t('navbar.services')}</Link>
              </li>
              <li className="navbar-item">
                <Link to="/contact" className={`navbar-link ${isActive('/contact') ? 'active' : ''}`}>{t('navbar.contact')}</Link>
              </li>
            </ul>
          </div>

          <div className="navbar-right">
            <div className="social-icons">
              <a href="https://www.facebook.com/SpaceLogicPro" className="social-icon" aria-label="Facebook" target="_blank" rel="noopener noreferrer">
                <i className="fab fa-facebook-f"></i>
              </a>
              <a href="https://x.com/SpaceLogicPro" className="social-icon" aria-label="X" target="_blank" rel="noopener noreferrer">
                <i className="fab fa-x-twitter"></i>
              </a>
              <a href="https://www.instagram.com/spacelogicpro/" className="social-icon" aria-label="Instagram" target="_blank" rel="noopener noreferrer">
                <i className="fab fa-instagram"></i>
              </a>
            </div>
            <div className="login-section">
              <Link to="/login" className="login-button">{t('navbar.login')}</Link>
              <LanguageSwitcher />
            </div>
          </div>

          <div className={`navbar-toggle ${isMobileMenuOpen ? 'active' : ''}`} onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}>
            <span className="bar"></span>
            <span className="bar"></span>
            <span className="bar"></span>
          </div>
        </div>
      </nav>
    </header>
  );
};

export default Navbar;
