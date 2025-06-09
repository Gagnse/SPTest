import React from 'react';
import { useTranslation } from 'react-i18next';

const About: React.FC = () => {
  const { t } = useTranslation('about');

  return (
    <div className="container page-content">
      <h1>{t('title')}</h1>
      <p>{t('teamIntro')}</p>

      <br />
      <h2>{t('missionTitle')}</h2>
      <p>{t('missionText')}</p>
    </div>
  );
};

export default About;
