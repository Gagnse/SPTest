import React from 'react';
import { useTranslation } from 'react-i18next';

const PrivacyPolicy: React.FC = () => {
  const { t } = useTranslation('privacy');

  return (
    <div className="container page-content">
      <h1>{t('title')}</h1>
      <p>{t('lastUpdated')}</p>

      <h3>{t('sections.intro.title')}</h3>
      <p>{t('sections.intro.text')}</p>

      <h3>{t('sections.collect.title')}</h3>
      <p>{t('sections.collect.text')}</p>

      <h3>{t('sections.use.title')}</h3>
      <p>{t('sections.use.text')}</p>

      <h3>{t('sections.share.title')}</h3>
      <p>{t('sections.share.text')}</p>

      <h3>{t('sections.security.title')}</h3>
      <p>{t('sections.security.text')}</p>

      <h3>{t('sections.retention.title')}</h3>
      <p>{t('sections.retention.text')}</p>

      <h3>{t('sections.age.title')}</h3>
      <p>{t('sections.age.text')}</p>

      <h3>{t('sections.changes.title')}</h3>
      <p>{t('sections.changes.text')}</p>

      <h3>{t('sections.storage.title')}</h3>
      <p>{t('sections.storage.text')}</p>

      <h3>{t('sections.contact.title')}</h3>
      <p>{t('sections.contact.text')}</p>
    </div>
  );
};

export default PrivacyPolicy;
