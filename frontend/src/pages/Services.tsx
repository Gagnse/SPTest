import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

const Services: React.FC = () => {
  const { t } = useTranslation('services');

  useEffect(() => {
    document.body.classList.add('page-theme-dark');
    return () => document.body.classList.remove('page-theme-dark');
  }, []);

  return (
    <div className="dark-page">
      <div className="container page-content" style={{ paddingBottom: '32px' }}>
        <header style={{ textAlign: 'center', marginBottom: '20px' }}>
          <h1 className="site-title" style={{ color: 'var(--ink)', marginBottom: 8 }}>{t('title')}</h1>
          <div className="accent-bar" style={{ margin: '20px auto 20px', width: '140px' }}></div>
          <p style={{ color: 'var(--grey)', marginTop: 12 }}>{t('intro')}</p>
        </header>

        <section className="cards-grid">
          <div className="service-card">
            <div className="chip chip-blue">{t('softwareTag')}</div>
            <h3 style={{color: 'var(--navy)', margin: '10px 0 6px', fontWeight: '600' }}>{t('softwareTitle')}</h3>
            <p style={{ color: 'var(--navy)' }}>{t('softwareText')}</p>
          </div>
          <div className="service-card">
            <div className="chip chip-mint">{t('implementationTag')}</div>
            <h3 style={{color: 'var(--navy)', margin: '10px 0 6px', fontWeight: '600' }}>{t('deploymentTitle')}</h3>
            <p style={{ color: 'var(--navy)' }}>{t('deploymentText')}</p>
          </div>
          <div className="service-card">
            <div className="chip chip-muted">{t('consultingTag')}</div>
            <h3 style={{color: 'var(--navy)', margin: '10px 0 6px', fontWeight: '600' }}>{t('advisoryTitle')}</h3>
            <p style={{ color: 'var(--navy)' }}>{t('advisoryText')}</p>
          </div>
          <div className="service-card">
            <div className="chip chip-blue">{t('integrationTag')}</div>
            <h3 style={{color: 'var(--navy)', margin: '10px 0 6px', fontWeight: '600' }}>{t('systemsTitle')}</h3>
            <p style={{ color: 'var(--navy)' }}>{t('systemsText')}</p>
          </div>
        </section>
      </div>
    </div>
  );
};

export default Services;
