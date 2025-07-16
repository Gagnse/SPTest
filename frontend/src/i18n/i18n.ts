import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import { resources, namespaces } from './resources';

i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: 'fr',
    fallbackLng: 'fr',
    ns: namespaces,
    defaultNS: 'common',
    interpolation: {
      escapeValue: false,
    },
  });

export default i18n;
