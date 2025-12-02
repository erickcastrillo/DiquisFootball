import React, { useState } from 'react';
import { Alert, AlertProps, Button, Col, Row } from 'react-bootstrap';

import PageLayout from 'components/PageLayout';
import SampleWrapper from 'template-resources/SampleWrapper';

const AlertDismissable = (props: Partial<AlertProps>) => {
  const [show, setShow] = useState(true);

  return (
    <Alert
      variant="danger"
      show={show}
      onClose={() => setShow(false)}
      dismissible
      {...props}
    >
      {props.children}
    </Alert>
  );
};

const Alerts = () => {
  const [show, setShow] = useState(false);

  return (
    <PageLayout title="Alerts">
      <Row>
        {/* Default Alerts */}
        <Col xl="6">
          <SampleWrapper
            title="Default Alerts"
            subText={
              <>
                {' '}
                Provide contextual feedback messages for typical user actions
                with the handful of available and flexible alert messages.
                Alerts are available for any length of text, as well as an
                optional dismiss button.{' '}
              </>
            }
          >
            <Alert show variant="primary">
              {' '}
              <strong>Primary - </strong> A simple primary alert—check it out!{' '}
            </Alert>
            <Alert show variant="secondary">
              {' '}
              <strong>Secondary - </strong> A simple secondary alert—check it
              out!{' '}
            </Alert>
            <Alert show variant="success">
              {' '}
              <strong>Success - </strong> A simple success alert—check it out!{' '}
            </Alert>
            <Alert show variant="danger">
              {' '}
              <strong>Error - </strong> A simple danger alert—check it out!{' '}
            </Alert>
            <Alert show variant="warning" className="text-bg-warning border-0">
              {' '}
              <strong>Warning - </strong> A simple warning alert—check it out!{' '}
            </Alert>
            <Alert show variant="info" className="text-bg-info border-0">
              {' '}
              <strong>Info - </strong> A simple info alert—check it out!{' '}
            </Alert>
            <Alert show variant="light" className="text-bg-light border-0">
              {' '}
              <strong>Light - </strong> A simple light alert—check it out!{' '}
            </Alert>
            <Alert show variant="dark" className="text-bg-dark border-0 mb-0">
              {' '}
              <strong>Dark - </strong> A simple dark alert—check it out!{' '}
            </Alert>
          </SampleWrapper>
        </Col>
        {/* Dismissing Alerts */}
        <Col xl="6">
          <SampleWrapper
            title="Dismissing Alerts"
            subText={
              <>
                Add a dismiss button and the <code>.alert-dismissible</code>{' '}
                class, which adds extra padding to the right of the alert and
                positions the <code>.btn-close</code> button.
              </>
            }
          >
            <AlertDismissable
              variant="primary"
              className="text-bg-primary border-0"
            >
              {' '}
              <strong>Primary - </strong> A simple primary alert—check it out!{' '}
            </AlertDismissable>
            <AlertDismissable
              variant="secondary"
              className="text-bg-secondary border-0"
            >
              {' '}
              <strong>Secondary - </strong> A simple secondary alert—check it
              out!{' '}
            </AlertDismissable>
            <AlertDismissable
              variant="success"
              className="text-bg-success border-0"
            >
              {' '}
              <strong>Success - </strong> A simple success alert—check it out!{' '}
            </AlertDismissable>
            <AlertDismissable
              variant="danger"
              className="text-bg-danger border-0"
            >
              {' '}
              <strong>Error - </strong> A simple danger alert—check it out!{' '}
            </AlertDismissable>
            <AlertDismissable
              variant="warning"
              className="text-bg-warning border-0"
            >
              {' '}
              <strong>Warning - </strong> A simple warning alert—check it out!{' '}
            </AlertDismissable>
            <AlertDismissable variant="info" className="text-bg-info border-0">
              {' '}
              <strong>Info - </strong> A simple info alert—check it out!{' '}
            </AlertDismissable>
            <AlertDismissable
              variant="light"
              className="text-bg-light border-0"
            >
              {' '}
              <strong>Light - </strong> A simple light alert—check it out!{' '}
            </AlertDismissable>
            <AlertDismissable
              variant="dark"
              className="text-bg-dark border-0 mb-0"
            >
              {' '}
              <strong>Dark - </strong> A simple dark alert—check it out!{' '}
            </AlertDismissable>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Custom Alerts */}
        <Col xl="6">
          <SampleWrapper
            title="Custom Alerts"
            subText={
              <>
                Display alert with transparent background and with contextual
                text color. Use classes
                <code>.bg-white</code>, and <code>.text-*</code>. E.g.{' '}
                <code>bg-white text-primary</code>.
              </>
            }
          >
            <Alert
              show
              variant="primary"
              className="bg-transparent text-primary"
            >
              {' '}
              This is a <strong>primary</strong> alert—check it out!{' '}
            </Alert>
            <Alert
              show
              variant="secondary"
              className="bg-transparent text-secondary"
            >
              {' '}
              This is a <strong>secondary</strong> alert—check it out!{' '}
            </Alert>
            <Alert
              show
              variant="success"
              className="bg-transparent text-success"
            >
              {' '}
              This is a <strong>success</strong> alert—check it out!{' '}
            </Alert>
            <Alert show variant="info" className="bg-transparent text-info">
              {' '}
              This is a <strong>info</strong> alert—check it out!{' '}
            </Alert>
            <Alert
              show
              variant="warning"
              className="bg-transparent text-warning"
            >
              {' '}
              This is a <strong>warning</strong> alert—check it out!{' '}
            </Alert>
            <Alert show variant="danger" className="bg-transparent text-danger">
              {' '}
              This is a <strong>danger</strong> alert—check it out!{' '}
            </Alert>
            <Alert show variant="light" className="bg-transparent text-light">
              {' '}
              This is a <strong>light</strong> alert—check it out!{' '}
            </Alert>
            <Alert
              show
              variant="dark"
              className="bg-transparent text-dark mb-0"
            >
              {' '}
              This is a <strong>dark</strong> alert—check it out!{' '}
            </Alert>
          </SampleWrapper>
        </Col>
        {/* Link Color */}
        <Col xl="6">
          <SampleWrapper
            title="Link Color"
            subText={
              <>
                Use the <code>.alert-link</code> utility class to quickly
                provide matching colored links within any alert.
              </>
            }
          >
            <Alert show variant="primary">
              A simple primary alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
            <Alert show variant="secondary">
              A simple secondary alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
            <Alert show variant="success">
              A simple success alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
            <Alert show variant="danger">
              A simple danger alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
            <Alert show variant="warning">
              A simple warning alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
            <Alert show variant="info">
              A simple info alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
            <Alert show variant="light">
              A simple light alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
            <Alert show variant="dark">
              A simple dark alert with
              {'  '}
              <a href="#" className="alert-link">
                an example link
              </a>
              . Give it a click if you like.
            </Alert>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Icons with Alerts */}
        <Col xl="6">
          <SampleWrapper
            title="Icons with Alerts"
            subText={
              <>
                You can also include additional elements like icons, heading,
                etc along side the actual message.
              </>
            }
          >
            <Alert show variant="success">
              {' '}
              <i className="ri-check-line me-1 align-middle fs-16"></i> This is
              a <strong>success</strong> alert - check it out!{' '}
            </Alert>
            <Alert show variant="danger">
              <i className="ri-close-circle-line me-1 align-middle fs-16"></i>{' '}
              This is a <strong>danger</strong>
              alert - check it out!
            </Alert>
            <Alert show variant="warning">
              {' '}
              <i className="ri-alert-line me-1 align-middle fs-16"></i> This is
              a <strong>warning</strong> alert - check it out!{' '}
            </Alert>
            <Alert show variant="info" className="mb-0">
              {' '}
              <i className="ri-information-line me-1 align-middle fs-16"></i>{' '}
              This is a <strong>info</strong> alert - check it out!{' '}
            </Alert>
          </SampleWrapper>
        </Col>
        {/* Additional Content */}
        <Col xl="6">
          <SampleWrapper
            title="Additional Content"
            subText={
              <>
                Alerts can also contain additional HTML elements like headings,
                paragraphs and dividers.
              </>
            }
          >
            <Alert show variant="info" className="text-center mb-0">
              <Alert.Heading>Well done!</Alert.Heading>
              <p>
                Aww yeah, you successfully read this important alert message.
                This example text is going to run a bit longer so that you can
                see how spacing within an alert works with this kind of content.
              </p>
              <hr className="border-info border-opacity-25" />
              <p className="mb-0">
                Whenever you need to, be sure to use margin utilities to keep
                things nice and tidy.
              </p>
            </Alert>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Live Alert */}
        <Col xl="6">
          <SampleWrapper
            title="Live Alert"
            subText={
              <>
                Click the button below to show an alert (hidden with inline
                styles to start), then dismiss (and destroy) it with the
                built-in close button.
              </>
            }
          >
            <Alert
              show={show}
              onClose={() => setShow(false)}
              dismissible
              variant="success"
            >
              <div>Nice, you triggered this alert message!</div>
            </Alert>
            <Button variant="primary" onClick={() => setShow((p) => !p)}>
              Show live alert
            </Button>
          </SampleWrapper>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default Alerts;
