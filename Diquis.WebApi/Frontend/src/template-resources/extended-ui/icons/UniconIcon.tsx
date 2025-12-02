import React from 'react';
import { Col, Row } from 'react-bootstrap';

import { PageLayout } from 'components';
import { unicons } from './data';

const UniconIcon = () => {
  return (
    <PageLayout title="Unicons">
      {Object.entries(unicons).map(([title, icons]) => (
        <div key={title} className="card">
          <div className="card-body">
            <h4 className="header-title mb-4">{title}</h4>
            <Row className="icons-list-demo">
              {icons.map((icon, index) => (
                <Col xl="3" lg="4" sm="6" key={index}>
                  <i className={`uil ${icon}`}></i>
                  {icon}
                </Col>
              ))}
            </Row>
          </div>
        </div>
      ))}
    </PageLayout>
  );
};

export default UniconIcon;
