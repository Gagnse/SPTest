import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

const About: React.FC = () => {
  const { t } = useTranslation('about');

  useEffect(() => {
    document.body.classList.add('page-theme-dark');
    return () => document.body.classList.remove('page-theme-dark');
  }, []);

  return (
    <div className="dark-page">
      <div className="container page-content" style={{ paddingBottom: '32px' }}>
        <header style={{ textAlign: 'center', marginBottom: '20px' }}>
          <h1 className="site-title" style={{ color: 'var(--ink)', marginBottom: 8 }}>{t('title')}</h1>
          <div className="accent-bar" style={{ margin: '20px auto', width: '140px' }}></div>
        </header>

        <div className="two-col">
          {/* About/company panel */}
          <section className="panel panel-glass">
            <h1 style={{ marginTop: 0, marginBottom: 40 }}>{t('title')}</h1>
            <p>{t('teamIntro')}</p>
            <div className="callout-on-dark" style={{ marginTop: 180 }}>
              <strong>{t('ethosTitle')}</strong>
              <p style={{ margin: '40px 0 0' }}>{t('missionText')}</p>
            </div>
          </section>

          {/* Missions/highlights panel */}
          <section className="panel panel-outline">
            <h2 style={{ marginTop: 0, marginBottom: 12 }}>{t('missionTitle')}</h2>
            <div className="cards-grid" style={{ marginTop: 8 }}>
              <div className="service-card">
                <div className="chip chip-blue">{t('missionChipArchitecture')}</div>
                <p style={{ marginTop: 8 }}>{t('missionText')}</p>
              </div>
              <div className="service-card">
                <div className="chip chip-mint">{t('missionChipLogistics')}</div>
                <p style={{ marginTop: 8 }}>{t('missionText')}</p>
              </div>
              <div className="service-card">
                <div className="chip chip-muted">{t('missionChipManufacturing')}</div>
                <p style={{ marginTop: 8 }}>{t('missionText')}</p>
              </div>
            </div>
          </section>
        </div>
      </div>
    </div>
  );
};

export default About;
