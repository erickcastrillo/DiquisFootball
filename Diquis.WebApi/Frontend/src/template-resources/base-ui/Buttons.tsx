import React from 'react';
import { Button, ButtonGroup, Col, Dropdown, Row } from 'react-bootstrap';

import PageLayout from 'components/PageLayout';
import SampleWrapper from 'template-resources/SampleWrapper';

const Buttons = () => {
  return (
    <PageLayout title="Buttons">
      <Row>
        {/* Default Buttons */}
        <Col xl="6">
          <SampleWrapper
            title="Default Buttons"
            subText={
              <>
                Use the button classes on an <code>&lt;a&gt;</code>,{' '}
                <code>&lt;button&gt;</code>, or <code>&lt;input&gt;</code>{' '}
                element.
              </>
            }
          >
            <div className="d-flex flex-wrap gap-2">
              <Button variant="primary">Primary</Button>
              <Button variant="secondary">Secondary</Button>
              <Button variant="success">Success</Button>
              <Button variant="danger">Danger</Button>
              <Button variant="warning">Warning</Button>
              <Button variant="info">Info</Button>
              <Button variant="light">Light</Button>
              <Button variant="dark">Dark</Button>
              <Button variant="link">Link</Button>
            </div>
          </SampleWrapper>
        </Col>
        {/* Button Outline */}
        <Col xl="6">
          <SampleWrapper
            title="Button Outline"
            subText={
              <>
                Use a classes <code>.btn-outline-**</code> to quickly create a
                bordered buttons.
              </>
            }
          >
            <div className="d-flex flex-wrap gap-2">
              <Button variant="outline-primary">Primary</Button>
              <Button variant="outline-secondary">Secondary</Button>
              <Button variant="outline-success">
                <i className="uil-cloud-computing"></i> Success
              </Button>
              <Button variant="outline-danger">Danger</Button>
              <Button variant="outline-warning">Warning</Button>
              <Button variant="outline-info">
                <i className="uil-circuit"></i> Info
              </Button>
              <Button variant="outline-light">Light</Button>
              <Button variant="outline-dark">Dark</Button>
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Button-Rounded */}
        <Col xl="6">
          <SampleWrapper
            title="Button-Rounded"
            subText={
              <>
                Add <code>.rounded-pill</code> to default button to get rounded
                corners.
              </>
            }
          >
            <div className="d-flex flex-wrap gap-2">
              <Button className="rounded-pill" variant="primary">
                Primary
              </Button>
              <Button className="rounded-pill" variant="secondary">
                Secondary
              </Button>
              <Button className="rounded-pill" variant="success">
                Success
              </Button>
              <Button className="rounded-pill" variant="danger">
                Danger
              </Button>
              <Button className="rounded-pill" variant="warning">
                Warning
              </Button>
              <Button className="rounded-pill" variant="info">
                Info
              </Button>
              <Button className="rounded-pill" variant="light">
                Light
              </Button>
              <Button className="rounded-pill" variant="dark">
                Dark
              </Button>
            </div>
          </SampleWrapper>
        </Col>
        {/* Button Outline Rounded */}
        <Col xl="6">
          <SampleWrapper
            title="Button Outline Rounded"
            subText={
              <>
                Use a classes <code>.btn-outline-**</code> to quickly create a
                bordered buttons.
              </>
            }
          >
            <div className="d-flex flex-wrap gap-2">
              <Button className="rounded-pill" variant="outline-primary">
                Primary
              </Button>
              <Button className="rounded-pill" variant="outline-secondary">
                Secondary
              </Button>
              <Button className="rounded-pill" variant="outline-success">
                <i className="uil-cloud-computing"></i> Success
              </Button>
              <Button className="rounded-pill" variant="outline-danger">
                Danger
              </Button>
              <Button className="rounded-pill" variant="outline-warning">
                Warning
              </Button>
              <Button className="rounded-pill" variant="outline-info">
                <i className="uil-circuit"></i> Info
              </Button>
              <Button className="rounded-pill" variant="outline-light">
                Light
              </Button>
              <Button className="rounded-pill" variant="outline-dark">
                Dark
              </Button>
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Soft Buttons */}
        <Col xl="6">
          <SampleWrapper
            title="Soft Buttons"
            subText={
              <>
                Use a classes <code>.btn-soft-**</code> to quickly create a soft
                background color buttons.
              </>
            }
          >
            <div className="d-flex flex-wrap gap-2">
              <Button variant="soft-primary">Primary</Button>
              <Button variant="soft-secondary">Secondary</Button>
              <Button variant="soft-success">Success</Button>
              <Button variant="soft-danger">Danger</Button>
              <Button variant="soft-warning">Warning</Button>
              <Button variant="soft-info">Info</Button>
              <Button variant="soft-dark">Dark</Button>
            </div>
          </SampleWrapper>
        </Col>
        {/* Soft Rounded Buttons */}
        <Col xl="6">
          <SampleWrapper
            title="Soft Rounded Buttons"
            subText={
              <>
                Use a classes <code>.btn-soft-**</code>{' '}
                <code>.rounded-pill</code> to quickly create a soft background
                color buttons with rounded.
              </>
            }
          >
            <div className="d-flex flex-wrap gap-2">
              <Button className="rounded-pill" variant="soft-primary">
                Primary
              </Button>
              <Button className="rounded-pill" variant="soft-secondary">
                Secondary
              </Button>
              <Button className="rounded-pill" variant="soft-success">
                Success
              </Button>
              <Button className="rounded-pill" variant="soft-danger">
                Danger
              </Button>
              <Button className="rounded-pill" variant="soft-warning">
                Warning
              </Button>
              <Button className="rounded-pill" variant="soft-info">
                Info
              </Button>
              <Button className="rounded-pill" variant="soft-dark">
                Dark
              </Button>
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Button-Sizes */}
        <Col xl="6">
          <SampleWrapper
            title="Button-Sizes"
            subText={
              <>
                Add <code>.btn-lg</code>, <code>.btn-sm</code> for additional
                sizes.
              </>
            }
          >
            <div className="d-flex flex-wrap align-items-center gap-2">
              <Button variant="primary" size="lg">
                Large
              </Button>
              <Button variant="info">Normal</Button>
              <Button variant="success" size="sm">
                Small
              </Button>
            </div>
          </SampleWrapper>
        </Col>
        {/* Button-Disabled */}
        <Col xl="6">
          <SampleWrapper
            title="Button-Disabled"
            subText={
              <>
                Add the <code>disabled</code> attribute to{' '}
                <code>&lt;button&gt;</code> buttons.
              </>
            }
          >
            <div className="d-flex flex-wrap gap-2">
              <Button variant="info" disabled>
                Info
              </Button>
              <Button variant="success" disabled>
                Success
              </Button>
              <Button variant="danger" disabled>
                Danger
              </Button>
              <Button variant="dark" disabled>
                Dark
              </Button>
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Icon Buttons */}
        <Col xl="6">
          <SampleWrapper title="Icon Buttons" subText={<>Icon only button.</>}>
            <div className="d-flex flex-wrap gap-2">
              <Button variant="light">
                <i className="mdi mdi-heart-outline"></i>{' '}
              </Button>
              <Button variant="danger">
                <i className="mdi mdi-window-close"></i>{' '}
              </Button>
              <Button variant="dark">
                <i className="mdi mdi-music"></i>{' '}
              </Button>
              <Button variant="primary">
                <i className="mdi mdi-star"></i>{' '}
              </Button>
              <Button variant="success">
                <i className="mdi mdi-thumb-up-outline"></i>{' '}
              </Button>
              <Button variant="info">
                <i className="mdi mdi-keyboard"></i>{' '}
              </Button>
              <Button variant="warning">
                <i className="mdi mdi-wrench"></i>{' '}
              </Button>

              <Button variant="light">
                <i className="mdi mdi-heart me-1"></i> <span>Like</span>{' '}
              </Button>
              <Button variant="warning">
                <i className="mdi mdi-rocket me-1"></i> <span>Launch</span>{' '}
              </Button>
              <Button variant="info">
                <i className="mdi mdi-cloud me-1"></i>{' '}
                <span>Cloud Hosting</span>{' '}
              </Button>

              <Button variant="outline-success">
                <i className="uil-money-withdrawal"></i> Money
              </Button>
              <Button variant="outline-primary">
                <i className="uil-paypal"></i> PayPal
              </Button>
              <Button variant="outline-danger">
                <i className="uil-cog"></i> Settings
              </Button>
            </div>
          </SampleWrapper>
        </Col>
        {/* Block Button */}
        <Col xl="6">
          <SampleWrapper
            title="Block Button"
            subText={
              <>
                Create block level buttons by adding class <code>.d-grid</code>{' '}
                to parent div.
              </>
            }
          >
            <div className="d-grid gap-2">
              <Button size="sm" variant="primary">
                Block Button
              </Button>
              <Button size="lg" variant="success">
                Block Button
              </Button>
            </div>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Button Group */}
        <Col xl="6">
          <SampleWrapper
            title="Button Group"
            subText={
              <>
                {' '}
                Wrap a series of buttons with <code>.btn</code> in{' '}
                <code>.btn-group</code>
              </>
            }
          >
            <ButtonGroup className="mb-2">
              <Button variant="light">Left</Button>
              <Button variant="light">Middle</Button>
              <Button variant="light">Right</Button>
            </ButtonGroup>

            <br />

            <ButtonGroup className="mb-2">
              <Button variant="light">1</Button>
              <Button variant="light">2</Button>
              <Button variant="light">3</Button>
              <Button variant="light">4</Button>
            </ButtonGroup>

            <ButtonGroup className="mb-2">
              <Button variant="light">5</Button>
              <Button variant="light">6</Button>
              <Button variant="light">7</Button>
            </ButtonGroup>

            <ButtonGroup className="mb-2">
              <Button variant="light">8</Button>
            </ButtonGroup>

            <br />

            <ButtonGroup className="mb-2">
              <Button variant="light">1</Button>
              <Button variant="primary">2</Button>
              <Button variant="light">3</Button>
              <ButtonGroup>
                <Dropdown>
                  <Dropdown.Toggle id="dropdown-basic-1" variant="light">
                    Dropdown
                  </Dropdown.Toggle>

                  <Dropdown.Menu>
                    <Dropdown.Item href="#">Dropdown link</Dropdown.Item>
                    <Dropdown.Item href="#">Dropdown link</Dropdown.Item>
                  </Dropdown.Menu>
                </Dropdown>
              </ButtonGroup>
            </ButtonGroup>

            <Row>
              <Col md="3">
                <ButtonGroup vertical className="mb-2">
                  <Button variant="light">Top</Button>
                  <Button variant="light">Middle</Button>
                  <Button variant="light">Bottom</Button>
                </ButtonGroup>
              </Col>
              <Col md="3">
                <ButtonGroup vertical className="mb-2">
                  <Button variant="light">Button 1</Button>
                  <Button variant="light">Button 2</Button>
                  <Dropdown>
                    <Dropdown.Toggle id="dropdown-basic-2" variant="light">
                      Button 3
                    </Dropdown.Toggle>

                    <Dropdown.Menu>
                      <Dropdown.Item href="#">Dropdown link</Dropdown.Item>
                      <Dropdown.Item href="#">Dropdown link</Dropdown.Item>
                    </Dropdown.Menu>
                  </Dropdown>
                </ButtonGroup>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
        <Col xl="6">
          <Row>
            {/* Toggle Button */}
            <Col xl="12">
              <SampleWrapper
                title="Toggle Button"
                subText={
                  <>
                    Add <code>data-bs-toggle="button"</code> to toggle a
                    button�s <code>active</code> state. If you�re pre-toggling a
                    button, you must manually add the <code>.active</code> class{' '}
                    <strong>and</strong> <code>aria-pressed="true"</code> to
                    ensure that it is conveyed appropriately to assistive
                    technologies.
                  </>
                }
              >
                <Button variant="primary" data-bs-toggle="button">
                  Toggle button
                </Button>
                <Button
                  variant="primary"
                  active
                  data-bs-toggle="button"
                  aria-pressed="true"
                >
                  Active toggle button
                </Button>
                <Button variant="primary" disabled data-bs-toggle="button">
                  Disabled toggle button
                </Button>
              </SampleWrapper>
            </Col>
            {/* Button tags */}
            <Col xl="12">
              <SampleWrapper
                title="Button tags"
                subText={
                  <>
                    The <code>.btn</code> classes are designed to be used with
                    the <code>&lt;button&gt;</code> element. However, you can
                    also use these classes on <code>&lt;a&gt;</code> or{' '}
                    <code>&lt;input&gt;</code> elements (though some browsers
                    may apply a slightly different rendering).
                  </>
                }
              >
                <div className="d-flex gap-1">
                  <a className="btn btn-primary" href="#" role="button">
                    Link
                  </a>
                  <Button variant="primary" type="submit">
                    Button
                  </Button>
                  <input
                    className="btn btn-primary"
                    type="button"
                    value="Input"
                  />
                  <input
                    className="btn btn-primary"
                    type="submit"
                    value="Submit"
                  />
                  <input
                    className="btn btn-primary"
                    type="reset"
                    value="Reset"
                  />
                </div>
              </SampleWrapper>
            </Col>
          </Row>
        </Col>
      </Row>
      <Row>
        {/* Basic Button */}
        <Col xl="6">
          <SampleWrapper
            title="Basic Button"
            subText={
              <>
                {' '}
                Bootstrap has a base <code>.btn</code> class that sets up basic
                styles such as padding and content alignment. By default,{' '}
                <code>.btn</code> controls have a transparent border and
                background color, and lack any explicit focus and hover styles.{' '}
              </>
            }
          >
            <button className="btn" type="button">
              Base class
            </button>
          </SampleWrapper>
        </Col>
        {/* Focus Ring Custom */}
        <Col xl="6">
          <SampleWrapper
            title="Focus Ring Custom"
            subText={
              <>
                Click directly on the link below to see the focus ring in
                action, or into the example below and then press <kbd>Tab</kbd>.
              </>
            }
          >
            <a
              href="#"
              className="d-inline-flex focus-ring py-1 px-2 text-decoration-none border rounded-2"
            >
              {' '}
              Custom focus ring{' '}
            </a>

            <a
              href="#"
              className="d-inline-flex focus-ring py-1 px-2 text-decoration-none border rounded-2"
              style={
                {
                  '--ct-focus-ring-color': 'rgba(var(--ct-success-rgb), 0.25)'
                } as any
              }
            >
              {' '}
              Green focus ring{' '}
            </a>

            <a
              href="#"
              className="d-inline-flex focus-ring py-1 px-2 text-decoration-none border rounded-2"
              style={
                {
                  '--ct-focus-ring-x': '10px',
                  '--ct-focus-ring-y': '10px',
                  '--ct-focus-ring-blur': '4px'
                } as any
              }
            >
              {' '}
              Blurry offset focus ring{' '}
            </a>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Focus Ring Utilities */}
        <Col xl="6">
          <SampleWrapper
            title="Focus Ring Utilities"
            subText={
              <>
                In addition to <code>.focus-ring</code>, we have several{' '}
                <code>.focus-ring-*</code> utilities to modify the helper class
                defaults. Modify the color with any of our{' '}
                <a href="ui-utilities.html">theme colors</a>. Note that the
                light and dark variants may not be visible on all background
                colors given current color mode support.
              </>
            }
          >
            <p>
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-primary py-1 px-2 text-decoration-none border rounded-2"
              >
                Primary focus
              </a>
            </p>
            <p>
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-secondary py-1 px-2 text-decoration-none border rounded-2"
              >
                Secondary focus
              </a>
            </p>
            <p>
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-success py-1 px-2 text-decoration-none border rounded-2"
              >
                Success focus
              </a>
            </p>
            <p>
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-danger py-1 px-2 text-decoration-none border rounded-2"
              >
                Danger focus
              </a>
            </p>
            <p>
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-warning py-1 px-2 text-decoration-none border rounded-2"
              >
                Warning focus
              </a>
            </p>
            <p>
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-info py-1 px-2 text-decoration-none border rounded-2"
              >
                Info focus
              </a>
            </p>
            <p>
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-light py-1 px-2 text-decoration-none border rounded-2"
              >
                Light focus
              </a>
            </p>
            <p className="mb-0">
              <a
                href="#"
                className="d-inline-flex focus-ring focus-ring-dark py-1 px-2 text-decoration-none border rounded-2"
              >
                Dark focus
              </a>
            </p>
          </SampleWrapper>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default Buttons;
