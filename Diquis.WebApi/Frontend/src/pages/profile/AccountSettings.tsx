import React, { useEffect, useState } from 'react';
import queryString from 'query-string';
import { Card, Col, Nav, Row } from 'react-bootstrap';
import { useLocation, useNavigate } from 'react-router-dom';

import { convertToSlug } from 'utils';
import { UserInfo, Password, Preferences } from './tabs';
import { useStore } from 'stores/store';

const AccountSettings = () => {
  const { authStore } = useStore();
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    authStore.setTitle('Profile');
  }, []);

  const [active, setActive] = useState('user-info');

  React.useEffect(() => {
    if (!location.search) return;
    const parsedQuery = queryString.parse(location.search);
    if (!parsedQuery.tab) return;
    setActive(parsedQuery.tab as string);
  }, [location]);

  return (
    <div className="pt-2 pb-4">
      <Row>
        <Col xxl={2} xl={3} lg={4}>
          <Card>
            <Card.Body className="px-2">
              {/* Side Menu */}
              <Nav variant="pills">
                {tabList.map(({ id, name, icon }) => (
                  <Nav.Item key={id} className="w-100">
                    <Nav.Link
                      active={active === convertToSlug(name)}
                      className="d-flex items-center"
                      onClick={() =>
                        navigate(`/profile?tab=${convertToSlug(name)}`)
                      }
                    >
                      <i className={`${icon} me-1`} />
                      <span className="">{name}</span>
                    </Nav.Link>
                  </Nav.Item>
                ))}
              </Nav>
            </Card.Body>
          </Card>
        </Col>

        {/* Tab Content */}
        <Col xxl={10} xl={9} lg={8}>
          {active === convertToSlug(tabList[0].name) && <UserInfo />}
          {active === convertToSlug(tabList[1].name) && <Preferences />}
          {active === convertToSlug(tabList[2].name) && <Password />}
        </Col>
      </Row>
    </div>
  );
};

const tabList = [
  {
    id: 1,
    name: 'User Info',
    icon: 'mdi mdi-account-circle'
  },
  {
    id: 2,
    name: 'Preferences',
    icon: 'mdi mdi-account-edit'
  },
  {
    id: 3,
    name: 'Password',
    icon: 'mdi mdi-lifebuoy'
  }
];

export default AccountSettings;
