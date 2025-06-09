import React from 'react';
import { useTranslation } from 'react-i18next';

const TermsOfService: React.FC = () => {
  const { t } = useTranslation('terms');

  return (
    <div className="container page-content">
      <h1>{t('title')}</h1>
      <p>{t('lastUpdated')}</p>

      <h2>{t('sections.intro.title')}</h2>
      <p>{t('sections.intro.text')}</p>

      <h3>{t('sections.acceptance.title')}</h3>
      <p>{t('sections.acceptance.text')}</p>

      <h3>{t('sections.changes.title')}</h3>
      <p>{t('sections.changes.text')}</p>

      <h3>{t('sections.use.title')}</h3>
      <p>{t('sections.use.text')}</p>

      <ul>
        <li>{t('sections.use.points.0')}</li>
        <li>{t('sections.use.points.1')}</li>
        <li>{t('sections.use.points.2')}</li>
        <li>{t('sections.use.points.3')}</li>
      </ul>

      <h3>{t('sections.account.title')}</h3>
      <p>{t('sections.account.text')}</p>

      <h3>{t('sections.termination.title')}</h3>
      <p>{t('sections.termination.text')}</p>

      <h3>{t('sections.liability.title')}</h3>
      <p>{t('sections.liability.text')}</p>

      <h3>{t('sections.contact.title')}</h3>
      <p>{t('sections.contact.text')}</p>
    </div>
  );
};

export default TermsOfService;
