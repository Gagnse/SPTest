import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';

const Contact: React.FC = () => {
  const { t } = useTranslation('contact');

  useEffect(() => {
    document.body.classList.add('page-theme-dark');
    return () => document.body.classList.remove('page-theme-dark');
  }, []);

  return (
    <div className="dark-page">
      <div className="container page-content" style={{ paddingBottom: '32px' }}>
        <header style={{ textAlign: 'center', marginBottom: '20px' }}>
          <h1 className="site-title" style={{ color: 'var(--ink)', marginBottom: 8 }}>{t('title')}</h1>
          <div className="accent-bar" style={{ margin: '20px auto', width: '120px' }}></div>
        </header>

        <div className="two-col">
          {/* Social links panel */}
          <section className="panel panel-glass">
            <h2 style={{ marginTop: 0 }}>{t('socialIntro')}</h2>
            <ul style={{ display: 'flex', gap: '10px', flexWrap: 'wrap', paddingLeft: 0, listStyle: 'none' }}>
              <li>
                <a className="chip chip-blue" href="https://www.instagram.com/spacelogicpro/" target="_blank" rel="noopener noreferrer">Instagram</a>
              </li>
              <li>
                <a className="chip chip-blue" href="https://x.com/SpaceLogicPro" target="_blank" rel="noopener noreferrer">Twitter/X</a>
              </li>
              <li>
                <a className="chip chip-blue" href="https://www.facebook.com/SpaceLogicPro" target="_blank" rel="noopener noreferrer">Facebook</a>
              </li>
            </ul>
          </section>

          {/* Message / email panel */}
          <section className="panel panel-outline">
            <h2 style={{ marginTop: 0 }}>{t('emailTitle')}</h2>
            <p style={{ color: 'var(--navy)' }}>{t('emailText')}</p>
            <div className="callout" style={{ marginTop: 12 }}>
              <strong>{t('ctaSpecificTitle')}</strong>
              <p style={{ margin: '6px 0 0' }}>{t('ctaSpecificBody')}</p>
            </div>
          </section>
        </div>
      </div>
    </div>
  );
};

export default Contact;
