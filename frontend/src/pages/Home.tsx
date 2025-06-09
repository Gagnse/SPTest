import React from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';

const Home: React.FC = () => {
  const navigate = useNavigate();
  const { t } = useTranslation('home');

  return (
    <div>
      {/* Hero Section */}
      <section className="parallax-hero">
        <div className="hero-content">
          <h1>{t('title')}</h1>
          <p>{t('description1')}</p>
          <p>{t('description2')}</p>
          <button className="cta-button" onClick={() => navigate('/projets')}>
            {t('buttons.getStarted')}
          </button>
        </div>
        <div className="scroll-down">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
            <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6z" />
          </svg>
        </div>
      </section>

      {/* Features Section */}
      <section className="features">
        <div className="features-container">
          <h2>{t('featuresTitle')}</h2>
          <div className="feature-grid">
            {[
              ['features.security'],
              ['features.simplicity'],
              ['features.history'],
              ['features.subscription'],
              ['features.comments']
            ].map(([key]) => (
              <div key={key} className="feature-card">
                <h3>{t(`${key}.title`)}</h3>
                <p>{t(`${key}.desc`)}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Why Choose Us Section */}
      <section className="parallax-section">
        <div className="parallax-bg"></div>
        <div className="parallax-content">
          <h2>{t('whyTitle')}</h2>
          <p>- {t('why1')}</p>
          <p>- {t('why2')}</p>
          <button className="cta-button" onClick={() => navigate('/services')}>
            {t('buttons.services')}
          </button>
        </div>
      </section>

      {/* About Section */}
      <section className="about">
        <div className="about-container">
          <h2>{t('aboutTitle')}</h2>
          <p>{t('aboutText')}</p>
          <p>
            {t('team.0')} <br />
            {t('team.1')} <br />
            {t('team.2')} <br />
          </p>
          <button className="cta-button" onClick={() => navigate('/about')}>
            {t('buttons.learnMore')}
          </button>
        </div>
      </section>
    </div>
  );
};

export default Home;