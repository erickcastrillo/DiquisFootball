import React from 'react';
import { Col, Row } from 'react-bootstrap';

import { PageLayout } from 'components';
import { remixIcons } from './data';

const RemixIcon = () => {
  return (
    <PageLayout title="Remix Icons">
      {Object.entries(remixIcons).map(([value, item]) => (
        <div className="card" key={value}>
          <div className="card-body">
            <h4 className="header-title">{value}</h4>
            <p className="card-title-desc mb-2">
              Use <code>&lt;i className="ri-home-line"&gt;&lt;/i&gt;</code> or
              <code>&lt;i className="ri-home-fill"&gt;&lt;/i&gt;</code>
            </p>

            {value === 'Editor' ? (
              <Row className="icons-list-demo">
                {Object.entries(item).map(([key]) => (
                  <Col xl="3" lg="4" sm="6" key={key}>
                    <i className={`ri-${key}`}></i>
                    <span>{`ri-${key}`}</span>
                  </Col>
                ))}
              </Row>
            ) : (
              <Row className="icons-list-demo">
                {Object.entries(item).map(([key]) => (
                  <React.Fragment key={key}>
                    <Col xl="3" lg="4" sm="6">
                      <i className={`ri-${key}-line`}></i>
                      <span>{`ri-${key}-line`}</span>
                    </Col>
                    <Col xl="3" lg="4" sm="6">
                      <i className={`ri-${key}-fill`}></i>
                      <span>{`ri-${key}-fill`}</span>
                    </Col>
                  </React.Fragment>
                ))}
              </Row>
            )}
          </div>
        </div>
      ))}
    </PageLayout>
  );
};

export default RemixIcon;
