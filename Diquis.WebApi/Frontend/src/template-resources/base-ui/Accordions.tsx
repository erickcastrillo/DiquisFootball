import React, { useContext } from 'react';
import clsx from 'clsx';
import {
  Accordion,
  AccordionContext,
  Button,
  Card,
  Col,
  Collapse,
  Row,
  useAccordionButton
} from 'react-bootstrap';
import { Link } from 'react-router-dom';

import PageLayout from 'components/PageLayout';
import SampleWrapper from 'template-resources/SampleWrapper';
import {
  AccordionItem,
  CustomAccordionProps,
  CustomToggleProps,
  SimpleCardAccordionCustomToggleProps
} from 'lib/types';
import { useToggle } from 'hooks';

const SimpleCardAccordionCustomToggle = ({
  eventKey,
  children
}: SimpleCardAccordionCustomToggleProps) => {
  const onClick = useAccordionButton(eventKey);

  return (
    <h5 className="m-0" onClick={onClick}>
      <a className="text-reset d-block">{children}</a>
    </h5>
  );
};

const CustomToggle = ({
  children,
  eventKey,
  containerClass,
  linkClass,
  callback
}: CustomToggleProps) => {
  const { activeEventKey } = useContext(AccordionContext);

  const decoratedOnClick = useAccordionButton(
    eventKey,
    () => callback && callback(eventKey)
  );

  const isCurrentEventKey = activeEventKey === eventKey;

  return (
    <h5 className={containerClass}>
      <Link
        to=""
        className={clsx(linkClass, {
          collapsed: !isCurrentEventKey
        })}
        onClick={decoratedOnClick}
      >
        {children}
      </Link>
    </h5>
  );
};

const CustomAccordion = ({ item, index }: CustomAccordionProps) => {
  return (
    <Card className="mb-0">
      <Card.Header>
        <CustomToggle
          eventKey={String(index)}
          containerClass="m-0"
          linkClass="custom-accordion-title d-block py-1"
        >
          Q. {item.title}
          <i className="mdi mdi-chevron-down accordion-arrow"></i>
        </CustomToggle>
      </Card.Header>
      <Accordion.Collapse eventKey={String(index)}>
        <div>
          <Card.Body>{item.text}</Card.Body>
        </div>
      </Accordion.Collapse>
    </Card>
  );
};

const Accordions = () => {
  const accordianContent: AccordionItem[] = [
    {
      id: 1,
      title: 'Can I use this template for my client?',
      text: ' Yup, the marketplace license allows you to use this theme in any end products. For more information on licenses, please refere'
    },
    {
      id: 2,
      title: 'Can this theme work with Wordpress?',
      text: "No. This is a HTML template. It won't directly with wordpress, though you can convert this into wordpress compatible theme"
    },
    {
      id: 3,
      title: 'How do I get help with the theme?',
      text: '   Use our dedicated support email to send your issues or feedback. We are here to help anytime'
    },
    {
      id: 4,
      title: 'Will you regularly give updates of UBold ?',
      text: 'Yes, We will update regularly. All the future updates would be available without any cost'
    }
  ];

  const [isOpen, toggle] = useToggle(true);
  const [isOpenFirst, toggleFirst] = useToggle(true);
  const [isOpenSecond, toggleSecond] = useToggle(true);
  const [isOpenHorizontal, toggleHorizontal] = useToggle(true);

  const toggleBoth = () => {
    toggleFirst();
    toggleSecond();
  };

  return (
    <PageLayout title="Accordions">
      <Row>
        {/* Default Accordions */}
        <Col xl="6">
          <SampleWrapper
            title="Default Accordions"
            subText=" Click the accordions below to expand/collapse the accordion content. "
          >
            <Accordion id="accordionExample" defaultActiveKey="0">
              <Accordion.Item eventKey="0">
                <Accordion.Header>Accordion Item #1</Accordion.Header>
                <Accordion.Body>
                  <div>
                    <strong>This is the first item's accordion body.</strong> It
                    is shown by default, until the collapse plugin adds the
                    appropriate classes that we use to style each element. These
                    classes control the overall appearance, as well as the
                    showing and hiding via CSS transitions. You can modify any
                    of this with custom CSS or overriding our default variables.
                    It's also worth noting that just about any HTML can go
                    within the <code>.accordion-body</code>, though the
                    transition does limit overflow.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
              <Accordion.Item eventKey="1">
                <Accordion.Header>Accordion Item #2</Accordion.Header>
                <Accordion.Body>
                  <div>
                    <strong>This is the second item's accordion body.</strong>{' '}
                    It is hidden by default, until the collapse plugin adds the
                    appropriate classes that we use to style each element. These
                    classes control the overall appearance, as well as the
                    showing and hiding via CSS transitions. You can modify any
                    of this with custom CSS or overriding our default variables.
                    It's also worth noting that just about any HTML can go
                    within the <code>.accordion-body</code>, though the
                    transition does limit overflow.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
              <Accordion.Item eventKey="2">
                <Accordion.Header>Accordion Item #3</Accordion.Header>
                <Accordion.Body>
                  <div>
                    <strong>This is the third item's accordion body.</strong> It
                    is hidden by default, until the collapse plugin adds the
                    appropriate classes that we use to style each element. These
                    classes control the overall appearance, as well as the
                    showing and hiding via CSS transitions. You can modify any
                    of this with custom CSS or overriding our default variables.
                    It's also worth noting that just about any HTML can go
                    within the <code>.accordion-body</code>, though the
                    transition does limit overflow.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
            </Accordion>
          </SampleWrapper>
        </Col>
        {/* Default Accordions Flush */}
        <Col xl="6">
          <SampleWrapper
            title="Default Accordions"
            subText={
              <>
                {' '}
                Add <code>.accordion-flush</code> to remove the default{' '}
                <code>background-color</code>, some borders, and some rounded
                corners to render accordions edge-to-edge with their parent
                container.{' '}
              </>
            }
          >
            <Accordion id="accordionFlushExample" flush>
              <Accordion.Item eventKey="0">
                <Accordion.Header>Accordion Item #1</Accordion.Header>
                <Accordion.Body>
                  <div>
                    Placeholder content for this accordion, which is intended to
                    demonstrate the <code>.accordion-flush</code> className.
                    This is the first item's accordion body.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
              <Accordion.Item eventKey="1">
                <Accordion.Header>Accordion Item #2</Accordion.Header>
                <Accordion.Body>
                  <div>
                    Placeholder content for this accordion, which is intended to
                    demonstrate the <code>.accordion-flush</code> class. This is
                    the second item's accordion body. Let's imagine this being
                    filled with some actual content.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
              <Accordion.Item eventKey="2">
                <Accordion.Header>Accordion Item #3</Accordion.Header>
                <Accordion.Body>
                  <div>
                    Placeholder content for this accordion, which is intended to
                    demonstrate the <code>.accordion-flush</code> class. This is
                    the third item's accordion body. Nothing more exciting
                    happening here in terms of content, but just filling up the
                    space to make it look, at least at first glance, a bit more
                    representative of how this would look in a real-world
                    application.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
            </Accordion>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Simple Card Accordions */}
        <Col xl="6">
          <SampleWrapper
            title="Simple Card Accordions"
            subText={
              <>
                Using the card component, you can extend the default collapse
                behavior to create an accordion. To properly achieve the
                accordion style, be sure to use <code>.accordion</code> as a
                wrapper.
              </>
            }
          >
            <Accordion>
              <Card>
                <Card.Header>
                  <SimpleCardAccordionCustomToggle eventKey="0">
                    {' '}
                    Collapsible Group Item #1{' '}
                  </SimpleCardAccordionCustomToggle>
                </Card.Header>
                <Accordion.Collapse eventKey="0">
                  <Card.Body className="pt-0">
                    Anim pariatur cliche reprehenderit, enim eiusmod high life
                    accusamus terry richardson ad squid. 3 wolf moon officia
                    aute, non cupidatat skateboard dolor brunch. Food truck
                    quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor,
                    sunt aliqua put a bird on it squid single-origin coffee
                    nulla assumenda shoreditch et. Nihil anim keffiyeh
                    helvetica, craft beer labore wes anderson cred nesciunt
                    sapiente ea proident. Ad vegan excepteur butcher vice lomo.
                    Leggings occaecat craft beer farm-to-table, raw denim
                    aesthetic synth nesciunt you probably haven't heard of them
                    accusamus labore susta inable VHS.
                  </Card.Body>
                </Accordion.Collapse>
              </Card>
              <Card>
                <Card.Header>
                  <SimpleCardAccordionCustomToggle eventKey="1">
                    {' '}
                    Collapsible Group Item #2{' '}
                  </SimpleCardAccordionCustomToggle>
                </Card.Header>
                <Accordion.Collapse eventKey="1">
                  <Card.Body className="pt-0">
                    Anim pariatur cliche reprehenderit, enim eiusmod high life
                    accusamus terry richardson ad squid. 3 wolf moon officia
                    aute, non cupidatat skateboard dolor brunch. Food truck
                    quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor,
                    sunt aliqua put a bird on it squid single-origin coffee
                    nulla assumenda shoreditch et. Nihil anim keffiyeh
                    helvetica, craft beer labore wes anderson cred nesciunt
                    sapiente ea proident. Ad vegan excepteur butcher vice lomo.
                    Leggings occaecat craft beer farm-to-table, raw denim
                    aesthetic synth nesciunt you probably haven't heard of them
                    accusamus labore susta inable VHS.
                  </Card.Body>
                </Accordion.Collapse>
              </Card>
              <Card>
                <Card.Header>
                  <SimpleCardAccordionCustomToggle eventKey="2">
                    {' '}
                    Collapsible Group Item #3{' '}
                  </SimpleCardAccordionCustomToggle>
                </Card.Header>
                <Accordion.Collapse eventKey="2">
                  <Card.Body className="pt-0">
                    Anim pariatur cliche reprehenderit, enim eiusmod high life
                    accusamus terry richardson ad squid. 3 wolf moon officia
                    aute, non cupidatat skateboard dolor brunch. Food truck
                    quinoa nesciunt laborum eiusmod. Brunch 3 wolf moon tempor,
                    sunt aliqua put a bird on it squid single-origin coffee
                    nulla assumenda shoreditch et. Nihil anim keffiyeh
                    helvetica, craft beer labore wes anderson cred nesciunt
                    sapiente ea proident. Ad vegan excepteur butcher vice lomo.
                    Leggings occaecat craft beer farm-to-table, raw denim
                    aesthetic synth nesciunt you probably haven't heard of them
                    accusamus labore susta inable VHS.
                  </Card.Body>
                </Accordion.Collapse>
              </Card>
            </Accordion>
          </SampleWrapper>
        </Col>
        {/* Always Open Accordions */}
        <Col xl="6">
          <SampleWrapper
            title="Always Open Accordions"
            subText={
              <>
                Omit the <code>data-bs-parent</code> attribute on each{'  '}
                <code>.accordion-collapse</code> to make accordion items stay
                open when another item is opened.
              </>
            }
          >
            <Accordion
              id="accordionPanelsStayOpenExample"
              defaultActiveKey={['0']}
              alwaysOpen
            >
              <Accordion.Item eventKey="0">
                <Accordion.Header>Accordion Item #1</Accordion.Header>
                <Accordion.Body>
                  <div>
                    <strong>This is the first item's accordion body.</strong> It
                    is shown by default, until the collapse plugin adds the
                    appropriate classes that we use to style each element. These
                    classes control the overall appearance, as well as the
                    showing and hiding via CSS transitions. You can modify any
                    of this with custom CSS or overriding our default variables.
                    It's also worth noting that just about any HTML can go
                    within the <code>.accordion-body</code>, though the
                    transition does limit overflow.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
              <Accordion.Item eventKey="1">
                <Accordion.Header>Accordion Item #2</Accordion.Header>
                <Accordion.Body>
                  <div>
                    <strong>This is the second item's accordion body.</strong>{' '}
                    It is hidden by default, until the collapse plugin adds the
                    appropriate classes that we use to style each element. These
                    classes control the overall appearance, as well as the
                    showing and hiding via CSS transitions. You can modify any
                    of this with custom CSS or overriding our default variables.
                    It's also worth noting that just about any HTML can go
                    within the <code>.accordion-body</code>, though the
                    transition does limit overflow.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
              <Accordion.Item eventKey="2">
                <Accordion.Header>Accordion Item #3</Accordion.Header>
                <Accordion.Body>
                  <div>
                    <strong>This is the third item's accordion body.</strong> It
                    is hidden by default, until the collapse plugin adds the
                    appropriate classes that we use to style each element. These
                    classes control the overall appearance, as well as the
                    showing and hiding via CSS transitions. You can modify any
                    of this with custom CSS or overriding our default variables.
                    It's also worth noting that just about any HTML can go
                    within the <code>.accordion-body</code>, though the
                    transition does limit overflow.
                  </div>
                </Accordion.Body>
              </Accordion.Item>
            </Accordion>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Custom Accordions */}
        <Col xl="6">
          <SampleWrapper
            title="Custom Accordions"
            subText={
              <>
                You can have custom look and feel for accorion as well. Just use
                class <code>.custom-accordion</code> along with
                <code>.accordion</code> as a wrapper.
              </>
            }
          >
            <Accordion
              className="custom-accordion"
              defaultActiveKey={['0']}
              id="custom-accordion"
              alwaysOpen
            >
              {(accordianContent || []).map((item, index) => {
                return (
                  <CustomAccordion
                    key={index.toString()}
                    item={item}
                    index={index}
                  />
                );
              })}
            </Accordion>
          </SampleWrapper>
        </Col>
        {/* Collapse */}
        <Col xl="6">
          <SampleWrapper
            title="Collapse"
            subText={
              <>
                Bootstrap's collapse provides the way to toggle the visibility
                of any content or element. Please read the official{' '}
                <a
                  href="https://getbootstrap.com/docs/5.2/components/collapse/"
                  target="_blank"
                >
                  Bootstrap
                </a>
                {'  '}
                documentation for a full list of options.
              </>
            }
          >
            <p>
              <Link to="" className="btn btn-primary" onClick={toggle}>
                Link with href
              </Link>

              <Button
                color="primary"
                className="ms-1"
                type="button"
                onClick={toggle}
              >
                Button with data-target
              </Button>
            </p>
            <Collapse in={isOpen}>
              <div>
                <div className="card card-body mb-0">
                  Anim pariatur cliche reprehenderit, enim eiusmod high life
                  accusamus terry richardson ad squid. Nihil anim keffiyeh
                  helvetica, craft beer labore wes anderson cred nesciunt
                  sapiente ea proident.
                </div>
              </div>
            </Collapse>
          </SampleWrapper>
        </Col>
      </Row>
      <Row>
        {/* Collapse Horizontal */}
        <Col xl="6">
          <SampleWrapper
            title="Collapse Horizontal"
            subText={
              <>
                Bootstrap's collapse provides the way to toggle the visibility
                of any content or element. Please read the official{' '}
                <a
                  href="https://getbootstrap.com/docs/5.2/components/collapse/"
                  target="_blank"
                >
                  Bootstrap
                </a>
                {'  '}
                documentation for a full list of options.
              </>
            }
          >
            <Button
              color="primary"
              className="ms-1"
              type="button"
              onClick={toggleHorizontal}
            >
              Toggle Width Collapse
            </Button>

            <div style={{ minHeight: '120px' }}>
              <Collapse in={isOpenHorizontal} dimension="width">
                <div id="example-collapse-text">
                  <Card body style={{ width: '300px' }}>
                    This is some placeholder content for a horizontal collapse.
                    It's hidden by default and shown when triggered.
                  </Card>
                </div>
              </Collapse>
            </div>
          </SampleWrapper>
        </Col>
        {/* Multiple Targets */}
        <Col xl="6">
          <SampleWrapper
            title="Multiple Targets"
            subText={
              <>
                Multiple <code>&lt;button&gt;</code> or <code>&lt;a&gt;</code>{' '}
                can show and hide an element if they each reference it with
                their <code>href</code>
                {'  '}or <code>data-bs-target</code> attribute.
              </>
            }
          >
            <p>
              <Link to="" className="btn btn-primary" onClick={toggleFirst}>
                Toggle first element
              </Link>

              <Button
                variant="primary"
                className="ms-1"
                type="button"
                onClick={toggleSecond}
              >
                Toggle second element
              </Button>

              <Button
                variant="primary"
                className="ms-1"
                type="button"
                onClick={toggleBoth}
              >
                Toggle both elements
              </Button>
            </p>
            <Row>
              <Col>
                <Collapse in={isOpenFirst}>
                  <div>
                    <div className="card card-body mb-0">
                      Anim pariatur cliche reprehenderit, enim eiusmod high life
                      accusamus terry richardson ad squid. Nihil anim keffiyeh
                      helvetica, craft beer labore wes anderson cred nesciunt
                      sapiente ea proident.
                    </div>
                  </div>
                </Collapse>
              </Col>
              <Col>
                <Collapse in={isOpenSecond}>
                  <div>
                    <div className="card card-body mb-0">
                      Anim pariatur cliche reprehenderit, enim eiusmod high life
                      accusamus terry richardson ad squid. Nihil anim keffiyeh
                      helvetica, craft beer labore wes anderson cred nesciunt
                      sapiente ea proident.
                    </div>
                  </div>
                </Collapse>
              </Col>
            </Row>
          </SampleWrapper>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default Accordions;
