import React, { useCallback, useEffect, useState } from 'react';
import { Col, Row } from 'react-bootstrap';

import { PageLayout } from 'components';
import { materialDesignIcons } from './data';

type Icon = { name: string; hex: string; version: string };

const MaterialDesignIcon = () => {
  const [newIcons, setNewIcons] = useState<Icon[]>([]);

  const isNew = useCallback((icon: Icon) => icon.version === '6.5.95', []);

  const getNewIcons = useCallback(() => {
    const _newIcons = [];
    for (let i in materialDesignIcons) {
      if (isNew(materialDesignIcons[i])) {
        _newIcons.push(materialDesignIcons[i]);
      }
    }
    setNewIcons(_newIcons);
  }, [isNew, materialDesignIcons]);

  useEffect(() => {
    getNewIcons();
  }, []);

  return (
    <PageLayout title="Material Design">
      <Row>
        <Col xs="12">
          <div className="card">
            <div className="card-body">
              <h4 className="header-title mb-4">
                New Icons{' '}
                <span className="badge badge-danger-lighten">6.5.95</span>
              </h4>
              <Row className="icons-list-demo" id="newIcons">
                {newIcons.map((icon, index) => (
                  <Col xl="3" lg="4" sm="6" key={index}>
                    {' '}
                    <i className={`mdi mdi-${icon.name}`}></i> mdi-{icon.name}{' '}
                  </Col>
                ))}
              </Row>
            </div>
          </div>
          <div className="card">
            <div className="card-body">
              <h4 className="header-title mb-4">All Icons</h4>
              <Row className="icons-list-demo" id="materialDesignIcons">
                {materialDesignIcons.map((icon, index) => (
                  <Col xl="3" lg="4" sm="6" key={index}>
                    {' '}
                    <i className={`mdi mdi-${icon.name}`}></i> mdi-{icon.name}{' '}
                  </Col>
                ))}
              </Row>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <div className="card">
            <div className="card-body">
              <h4 className="header-title mb-4">Size</h4>
              <Row className="icons-list-demo">
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-18px mdi-account"></i> mdi-18px{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-24px mdi-account"></i> mdi-24px{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-36px mdi-account"></i> mdi-36px{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-48px mdi-account"></i> mdi-48px{' '}
                </Col>
              </Row>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <div className="card">
            <div className="card-body">
              <h4 className="header-title mb-4">Rotate</h4>
              <Row className="icons-list-demo">
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-rotate-45 mdi-account"></i>{' '}
                  mdi-rotate-45{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-rotate-90 mdi-account"></i>{' '}
                  mdi-rotate-90{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-rotate-135 mdi-account"></i>{' '}
                  mdi-rotate-135{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-rotate-180 mdi-account"></i>{' '}
                  mdi-rotate-180{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-rotate-225 mdi-account"></i>{' '}
                  mdi-rotate-225{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-rotate-270 mdi-account"></i>{' '}
                  mdi-rotate-270{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-rotate-315 mdi-account"></i>{' '}
                  mdi-rotate-315{' '}
                </Col>
              </Row>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <div className="card">
            <div className="card-body">
              <h4 className="header-title mb-4">Spin</h4>
              <Row className="icons-list-demo">
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-spin mdi-loading"></i> mdi-spin{' '}
                </Col>
                <Col xl="3" lg="4" sm="6">
                  {' '}
                  <i className="mdi mdi-spin mdi-star"></i> mdi-spin{' '}
                </Col>
              </Row>
            </div>
          </div>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default MaterialDesignIcon;
