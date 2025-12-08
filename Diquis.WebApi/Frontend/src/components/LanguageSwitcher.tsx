import React from 'react';
import { useTranslation } from 'react-i18next';
import { Dropdown } from 'react-bootstrap';

import usFlag from 'assets/images/flags/us.jpg';
import spainFlag from 'assets/images/flags/spain.jpg';

const LanguageSwitcher: React.FC = () => {
  const { i18n } = useTranslation();

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
  };

  const getFlag = (lng: string) => {
    if (lng === 'es') {
      return spainFlag;
    } else {
      return usFlag; // Default to US flag for English
    }
  };

  const getCurrentLanguageName = (lng: string) => {
    if (lng === 'es') {
      return 'Español';
    } else {
      return 'English';
    }
  };

  return (
    <Dropdown align="end">
      <Dropdown.Toggle variant="link" id="dropdown-languages" className="nav-link dropdown-toggle">
        <img src={getFlag(i18n.language)} alt="flag" className="me-1" height="12" />
        <span className="align-middle d-none d-sm-inline-block">{getCurrentLanguageName(i18n.language)}</span>
      </Dropdown.Toggle>

      <Dropdown.Menu className="dropdown-menu-end">
        <Dropdown.Item onClick={() => changeLanguage('es')} className="notify-item">
          <img src={spainFlag} alt="flag" className="me-1" height="12" />
          <span className="align-middle">Español</span>
        </Dropdown.Item>
        <Dropdown.Item onClick={() => changeLanguage('en')} className="notify-item">
          <img src={usFlag} alt="flag" className="me-1" height="12" />
          <span className="align-middle">English</span>
        </Dropdown.Item>
      </Dropdown.Menu>
    </Dropdown>
  );
};

export default LanguageSwitcher;
