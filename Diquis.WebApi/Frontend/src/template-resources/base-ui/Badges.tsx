import React from 'react';
import { Badge, Button, Col, Row } from 'react-bootstrap';

import PageLayout from 'components/PageLayout';
import SampleWrapper from 'template-resources/SampleWrapper';

const Badges = () => {
  return (
    <PageLayout title="Badges">
      <Row>
        <Col xl="6">
          {/* Default */}
          <SampleWrapper
            title="Default"
            subText={
              <>
                A simple labeling component. Badges scale to match the size of
                the immediate parent element by using relative font sizing and{' '}
                <code>em</code> units.
              </>
            }
          >
            <h1>
              h1.Example heading <Badge bg="secondary">New</Badge>
            </h1>
            <h2>
              h2.Example heading{' '}
              <Badge className="bg-success-subtle text-success">New</Badge>
            </h2>
            <h3>
              h2.Example heading <Badge bg="primary">New</Badge>
            </h3>
            <h4>
              h4.Example heading{' '}
              <Badge className="bg-info-subtle text-info">Info Link</Badge>
            </h4>
            <h5>
              h5.Example heading{' '}
              <Badge bg="text-warning" className="badge-outline-warning">
                New
              </Badge>
            </h5>
            <h6>
              h6.Example heading <Badge className="bg-danger">New</Badge>
            </h6>
          </SampleWrapper>
          {/* Pill Badges */}
          <SampleWrapper
            title="Pill Badges"
            subText={
              <>
                Use the <code>.rounded-pill</code> modifier class to make badges
                more rounded.
              </>
            }
          >
            <Badge pill bg="primary">
              Primary
            </Badge>
            <Badge pill className="text-light ms-1" bg="secondary">
              Secondary
            </Badge>
            <Badge pill bg="success" className="ms-1">
              Success
            </Badge>
            <Badge pill className="bg-danger ms-1">
              Danger
            </Badge>
            <Badge pill className="bg-warning ms-1">
              Warning
            </Badge>
            <Badge pill className="bg-info ms-1">
              Info
            </Badge>
            <Badge pill bg="light" className="ms-1">
              Light
            </Badge>
            <Badge pill bg="dark" className="ms-1">
              Dark
            </Badge>

            <h5 className="mt-4">Lighten Badges</h5>
            <p className="text-muted font-14 mb-3">
              Use the <code>.badge-*-lighten</code> modifier class to make
              badges lighten.
            </p>

            <Badge pill className="bg-primary-subtle text-primary">
              Primary
            </Badge>
            <Badge pill className="bg-secondary-subtle text-secondary ms-1">
              Secondary
            </Badge>
            <Badge pill className="bg-success-subtle text-success ms-1">
              Success
            </Badge>
            <Badge pill className="bg-danger-subtle text-danger ms-1">
              Danger
            </Badge>
            <Badge pill className="bg-warning-subtle text-warning ms-1">
              Warning
            </Badge>
            <Badge pill className="bg-info-subtle text-info ms-1">
              Info
            </Badge>
            <Badge pill className="bg-dark-subtle text-dark ms-1">
              Dark
            </Badge>

            <h5 className="mt-4">Outline Badges</h5>
            <p className="text-muted font-14 mb-3">
              Using the <code>.badge-outline-*</code> to quickly create a
              bordered badges.
            </p>

            <Badge pill className="badge-outline-primary" bg="text-primary">
              Primary
            </Badge>
            <Badge
              pill
              className="badge-outline-secondary ms-1"
              bg="text-secondary"
            >
              Secondary
            </Badge>
            <Badge
              pill
              className="badge-outline-success ms-1"
              bg="text-success"
            >
              Success
            </Badge>
            <Badge pill className="badge-outline-danger ms-1" bg="text-danger">
              Danger
            </Badge>
            <Badge
              pill
              className="badge-outline-warning ms-1"
              bg="text-warning"
            >
              Warning
            </Badge>
            <Badge pill className="badge-outline-info ms-1" bg="text-info">
              Info
            </Badge>
            <Badge pill className="badge-outline-dark ms-1" bg="text-dark">
              Dark
            </Badge>
          </SampleWrapper>
        </Col>
        <Col xl="6">
          {/* Contextual variations */}
          <SampleWrapper
            title="Contextual variations"
            subText={
              <>
                Add any of the below mentioned modifier classes to change the
                appearance of a badge. Badge can be more contextual as well.
                Just use regular convention e.g. <code>badge-*color</code>,{' '}
                <code>bg-primary</code>
                to have badge with different background.
              </>
            }
          >
            <Badge bg="primary">Primary</Badge>
            <Badge className="bg-secondary ms-1">Secondary</Badge>
            <Badge className="bg-success ms-1">Success</Badge>
            <Badge className="bg-danger ms-1">Danger</Badge>
            <Badge className="bg-warning ms-1">Warning</Badge>
            <Badge className="bg-info ms-1">Info</Badge>
            <Badge bg="light" className="text-dark ms-1">
              Light
            </Badge>
            <Badge bg="dark" className="text-light ms-1">
              Dark
            </Badge>
            <h5 className="mt-4">Lighten Badges</h5>
            <p className="text-muted font-14 mb-3">
              Using the <code>.badge-*-lighten</code> modifier class, you can
              have more soften variation.
            </p>

            <Badge className="bg-primary-subtle text-primary">Primary</Badge>
            <Badge className="bg-secondary-subtle text-secondary ms-1">
              Secondary
            </Badge>
            <Badge className="bg-success-subtle text-success ms-1">
              Success
            </Badge>
            <Badge className="bg-danger-subtle text-danger ms-1">Danger</Badge>
            <Badge className="bg-warning-subtle text-warning ms-1">
              Warning
            </Badge>
            <Badge className="bg-info-subtle text-info ms-1">Info</Badge>
            <Badge className="bg-dark-subtle text-dark ms-1">Dark</Badge>

            <h5 className="mt-4">Outline Badges</h5>
            <p className="text-muted font-14 mb-3">
              Using the <code>.badge-outline-*</code> to quickly create a
              bordered badges.
            </p>

            <Badge className="badge-outline-primary" bg="text-primary">
              Primary
            </Badge>
            <Badge className="badge-outline-secondary ms-1" bg="text-secondary">
              Secondary
            </Badge>
            <Badge className="badge-outline-success ms-1" bg="text-success">
              Success
            </Badge>
            <Badge className="badge-outline-danger ms-1" bg="text-danger">
              Danger
            </Badge>
            <Badge className="badge-outline-warning ms-1" bg="text-warning">
              Warning
            </Badge>
            <Badge className="badge-outline-info ms-1" bg="text-info">
              Info
            </Badge>
            <Badge className="badge-outline-dark ms-1" bg="text-dark">
              Dark
            </Badge>
          </SampleWrapper>
          {/* Badge Positioned */}
          <SampleWrapper
            title="Badge Positioned"
            subText={
              <>
                Use utilities to modify a <code>.badge</code> and position it in
                the corner of a link or button.
              </>
            }
          >
            <Row>
              <Col xs="6">
                <Button className="position-relative" variant="primary">
                  Inbox
                  <Badge
                    pill
                    className="position-absolute top-0 start-100 translate-middle bg-danger"
                  >
                    99+
                    <span className="visually-hidden">unread messages</span>
                  </Badge>
                </Button>
              </Col>
              <Col xs="6">
                <Button className="position-relative" variant="primary">
                  Profile
                  <Badge className="position-absolute top-0 start-100 translate-middle p-1 bg-danger border border-light rounded-circle">
                    <span className="visually-hidden">New alerts</span>
                  </Badge>
                </Button>
              </Col>
              <Col xs="6">
                <Button className="mt-4" variant="success">
                  {' '}
                  Notifications{' '}
                  <Badge className="bg-light text-dark ms-1">4</Badge>{' '}
                </Button>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default Badges;
