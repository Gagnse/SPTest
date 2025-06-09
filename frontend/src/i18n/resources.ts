import commonEN from './locales/en/common.json';
import homeEN from './locales/en/home.json';
import aboutEN from './locales/en/about.json';
import servicesEN from './locales/en/services.json';
import contactEN from './locales/en/contact.json';
import loginEN from './locales/en/auth/login.json';
import termsEN from './locales/en/terms.json';

import commonFR from './locales/fr/common.json';
import homeFR from './locales/fr/home.json';
import aboutFR from './locales/fr/about.json';
import servicesFR from './locales/fr/services.json';
import contactFR from './locales/fr/contact.json';
import loginFR from './locales/fr/auth/login.json';
import privacyEN from './locales/en/privacy.json';
import privacyFR from './locales/fr/privacy.json';
import termsFR from './locales/fr/terms.json';

export const resources = {
  en: {
    common: commonEN,
    home: homeEN,
    about: aboutEN,
    services: servicesEN,
    contact: contactEN,
    privacy: privacyEN,
    terms: termsEN,
    'auth/login': loginEN
  },
  fr: {
    common: commonFR,
    home: homeFR,
    about: aboutFR,
    services: servicesFR,
    contact: contactFR,
    privacy: privacyFR,
    terms: termsFR,
    'auth/login': loginFR
  }
};

export const namespaces = [
  'common',
  'home',
  'about',
  'services',
  'contact',
  'privacy',
  'terms',
  'auth/login'
];
