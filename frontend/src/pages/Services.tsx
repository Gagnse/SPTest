import React from 'react';
import { useTranslation } from 'react-i18next';

const Services: React.FC = () => {
  const { t } = useTranslation('services');

  return (
    <div className="container page-content">
      <h1>{t('title')}</h1>
      <p>{t('intro')}</p>

      <br />
      <h2>{t('softwareTitle')}</h2>
      <p>{t('softwareText')}</p>
    </div>
  );
};

export default Services;
