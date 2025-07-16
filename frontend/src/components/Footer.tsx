import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

const Footer: React.FC = () => {
  const { t } = useTranslation('common');

  return (
    <footer>
      <div className="footer-container">
        <p>{t('footer.copyright')}</p>
        <div className="footer-links">
          <Link to="/privacy">{t('footer.privacy')}</Link>
          <Link to="/terms">{t('footer.terms')}</Link>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
