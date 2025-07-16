import React from 'react';
import { useTranslation } from 'react-i18next';

const Contact: React.FC = () => {
  const { t } = useTranslation('contact');

  return (
    <div className="container page-content">
      <h1>{t('title')}</h1>

      <p>{t('socialIntro')}</p>
      <br />
      <ul>
        <li>
          <strong><a href="https://www.instagram.com/spacelogicpro/" target="_blank" rel="noopener noreferrer">Instagram</a></strong>
        </li>
        <li>
          <strong><a href="https://x.com/SpaceLogicPro" target="_blank" rel="noopener noreferrer">Twitter/X</a></strong>
        </li>
        <li>
          <strong><a href="https://www.facebook.com/SpaceLogicPro" target="_blank" rel="noopener noreferrer">Facebook</a></strong>
        </li>
      </ul>

      <br />
      <h2>{t('emailTitle')}</h2>
      <p>{t('emailText')}</p>
    </div>
  );
};

export default Contact;
