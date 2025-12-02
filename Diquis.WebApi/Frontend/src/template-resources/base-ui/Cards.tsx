import React from 'react';
import { Col, Row } from 'react-bootstrap';

import PageLayout from 'components/PageLayout';
import small1 from 'assets/images/small/small-1.jpg';
import small2 from 'assets/images/small/small-2.jpg';
import small3 from 'assets/images/small/small-3.jpg';
import small4 from 'assets/images/small/small-4.jpg';

const Cards = () => {
  return (
    <PageLayout title="Cards">
      <Row>
        {/* Simple card */}
        <Col sm="6" lg="3">
          <div className="card d-block">
            <img className="card-img-top" src={small1} alt="Card image cap" />
            <div className="card-body">
              <h5 className="card-title">Card title</h5>
              <p className="card-text">
                Some quick example text to build on the card title and make up
                the bulk of the card's content. Some quick example text to build
                on the card title and make up.
              </p>
              <a href="#" className="btn btn-primary">
                Button
              </a>
            </div>
          </div>
        </Col>
        <Col sm="6" lg="3">
          <div className="card d-block">
            <img className="card-img-top" src={small2} alt="Card image cap" />
            <div className="card-body">
              <h5 className="card-title">Card title</h5>
              <p className="card-text">
                Some quick example text to build on the card..
              </p>
            </div>
            <ul className="list-group list-group-flush">
              <li className="list-group-item">Cras justo odio</li>
            </ul>
            <div className="card-body">
              <a href="#" className="card-link text-custom">
                Card link
              </a>
              <a href="#" className="card-link text-custom">
                Another link
              </a>
            </div>
          </div>
        </Col>
        <Col sm="6" lg="3">
          <div className="card d-block">
            <img className="card-img-top" src={small3} alt="Card image cap" />
            <div className="card-body">
              <p className="card-text">
                Some quick example text to build on the card title and make up
                the bulk of the card's content. Some quick example text to build
                on the card title and make up.
              </p>
              <a href="#" className="btn btn-primary">
                Button
              </a>
            </div>
          </div>
        </Col>
        <Col sm="6" lg="3">
          <div className="card d-block">
            <div className="card-body">
              <h5 className="card-title">Card title</h5>
              <h6 className="card-subtitle text-muted">
                Support card subtitle
              </h6>
            </div>
            <img className="img-fluid" src={small4} alt="Card image cap" />
            <div className="card-body">
              <p className="card-text">
                Some quick example text to build on the card title and make up
                the bulk of the card's content.
              </p>
              <a href="#" className="card-link text-custom">
                Card link
              </a>
              <a href="#" className="card-link text-custom">
                Another link
              </a>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col sm="6">
          <div className="card card-body">
            <h5 className="card-title">Special title treatment</h5>
            <p className="card-text">
              With supporting text below as a natural lead-in to additional
              content.
            </p>
            <a href="#" className="btn btn-primary">
              Go somewhere
            </a>
          </div>
        </Col>
        <Col sm="6">
          <div className="card card-body">
            <h5 className="card-title">Special title treatment</h5>
            <p className="card-text">
              With supporting text below as a natural lead-in to additional
              content.
            </p>
            <a href="#" className="btn btn-primary">
              Go somewhere
            </a>
          </div>
        </Col>
      </Row>
      <Row>
        <Col md="4">
          <div className="card">
            <h6 className="card-header bg-light">Featured</h6>
            <div className="card-body">
              <h5 className="card-title">Special title treatment</h5>
              <p className="card-text">
                With supporting text below as a natural lead-in to additional
                content.
              </p>
              <a href="#" className="btn btn-primary">
                Go somewhere
              </a>
            </div>
          </div>
        </Col>
        <Col md="4">
          <div className="card">
            <div className="card-header bg-light">Quote</div>
            <div className="card-body">
              <blockquote className="card-bodyquote">
                <p>
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                  Integer posuere erat a ante.
                </p>
                <footer>
                  Someone famous in{' '}
                  <cite title="Source Title">Source Title</cite>
                </footer>
              </blockquote>
            </div>
          </div>
        </Col>
        <Col md="4">
          <div className="card">
            <div className="card-header bg-light">Featured</div>
            <div className="card-body">
              <a href="#" className="btn btn-primary">
                Go somewhere
              </a>
            </div>
            <div className="card-footer border-top border-light text-muted">
              2 days ago
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <h4 className="mb-4">Card Colored</h4>
        </Col>
      </Row>
      {/* Cards Colored */}
      <Row>
        <Col lg="4" sm="6">
          <div className="card text-bg-secondary">
            <div className="card-body">
              <h5 className="card-title">Special title treatment</h5>
              <p className="card-text">
                With supporting text below as a natural lead-in to additional
                content.
              </p>
              <a href="#" className="btn btn-primary btn-sm">
                Button
              </a>
            </div>
          </div>
        </Col>
        <Col lg="4" sm="6">
          <div className="card text-bg-primary">
            <div className="card-body">
              <blockquote className="card-bodyquote">
                <p>
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                  Integer posuere erat a ante.
                </p>
                <footer>
                  Someone famous in{' '}
                  <cite title="Source Title">Source Title</cite>
                </footer>
              </blockquote>
            </div>
          </div>
        </Col>
        <Col lg="4" sm="6">
          <div className="card text-bg-success">
            <div className="card-body">
              <blockquote className="card-bodyquote">
                <p>
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                  Integer posuere erat a ante.
                </p>
                <footer>
                  Someone famous in{' '}
                  <cite title="Source Title">Source Title</cite>
                </footer>
              </blockquote>
            </div>
          </div>
        </Col>
        <Col lg="4" sm="6">
          <div className="card text-bg-info">
            <div className="card-body">
              <blockquote className="card-bodyquote mb-0">
                <p>
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                  Integer posuere erat a ante.
                </p>
                <footer>
                  Someone famous in{' '}
                  <cite title="Source Title">Source Title</cite>
                </footer>
              </blockquote>
            </div>
          </div>
        </Col>
        <Col lg="4" sm="6">
          <div className="card text-bg-warning">
            <div className="card-body">
              <blockquote className="card-bodyquote mb-0">
                <p>
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                  Integer posuere erat a ante.
                </p>
                <footer>
                  Someone famous in{' '}
                  <cite title="Source Title">Source Title</cite>
                </footer>
              </blockquote>
            </div>
          </div>
        </Col>
        <Col lg="4" sm="6">
          <div className="card text-bg-danger">
            <div className="card-body">
              <blockquote className="card-bodyquote mb-0">
                <p>
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit.
                  Integer posuere erat a ante.
                </p>
                <footer>
                  Someone famous in{' '}
                  <cite title="Source Title">Source Title</cite>
                </footer>
              </blockquote>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <h4 className="mb-4">Card Bordered</h4>
        </Col>
      </Row>
      {/* Cards Bordered */}
      <Row>
        <Col md="4">
          <div className="card border-secondary border">
            <div className="card-body">
              <h5 className="card-title">Special title treatment</h5>
              <p className="card-text">
                With supporting text below as a natural lead-in to additional
                content.
              </p>
              <a href="#" className="btn btn-secondary btn-sm">
                Button
              </a>
            </div>
          </div>
        </Col>
        <Col md="4">
          <div className="card border-primary border">
            <div className="card-body">
              <h5 className="card-title text-primary">
                Special title treatment
              </h5>
              <p className="card-text">
                With supporting text below as a natural lead-in to additional
                content.
              </p>
              <a href="#" className="btn btn-primary btn-sm">
                Button
              </a>
            </div>
          </div>
        </Col>
        <Col md="4">
          <div className="card border-success border">
            <div className="card-body">
              <h5 className="card-title text-success">
                Special title treatment
              </h5>
              <p className="card-text">
                With supporting text below as a natural lead-in to additional
                content.
              </p>
              <a href="#" className="btn btn-success btn-sm">
                Button
              </a>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <h4 className="mb-4">Horizontal Card</h4>
        </Col>
      </Row>
      {/* Horizontal Card */}
      <Row>
        <Col lg="6">
          <div className="card">
            <div className="row g-0 align-items-center">
              <div className="col-md-4">
                <img
                  src={small4}
                  className="img-fluid rounded-start"
                  alt="..."
                />
              </div>
              <div className="col-md-8">
                <div className="card-body">
                  <h5 className="card-title">Card title</h5>
                  <p className="card-text">
                    This is a wider card with supporting text below as a natural
                    lead-in to additional content. This content is a little bit
                    longer.
                  </p>
                  <p className="card-text">
                    <small className="text-muted">
                      Last updated 3 mins ago
                    </small>
                  </p>
                </div>
              </div>
            </div>
          </div>
        </Col>
        <Col lg="6">
          <div className="card">
            <div className="row g-0 align-items-center">
              <div className="col-md-8">
                <div className="card-body">
                  <h5 className="card-title">Card title</h5>
                  <p className="card-text">
                    This is a wider card with supporting text below as a natural
                    lead-in to additional content. This content is a little bit
                    longer.
                  </p>
                  <p className="card-text">
                    <small className="text-muted">
                      Last updated 3 mins ago
                    </small>
                  </p>
                </div>
              </div>

              <div className="col-md-4">
                <img src={small1} className="img-fluid rounded-end" alt="..." />
              </div>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <h4 className="mb-4">Stretched Link</h4>
        </Col>
      </Row>
      {/* Stretched Link */}
      <Row>
        <Col sm="6" lg="3">
          <div className="card">
            <img src={small2} className="card-img-top" alt="..." />
            <div className="card-body">
              <h5 className="card-title">Card with stretched link</h5>
              <a href="#" className="btn btn-primary mt-2 stretched-link">
                Go somewhere
              </a>
            </div>
          </div>
        </Col>
        <Col sm="6" lg="3">
          <div className="card">
            <img src={small3} className="card-img-top" alt="..." />
            <div className="card-body">
              <h5 className="card-title">
                <a href="#" className="text-success stretched-link">
                  Card with stretched link
                </a>
              </h5>
              <p className="card-text">
                Some quick example text to build on the card up the bulk of the
                card's content.
              </p>
            </div>
          </div>
        </Col>
        <Col sm="6" lg="3">
          <div className="card">
            <img src={small4} className="card-img-top" alt="..." />
            <div className="card-body">
              <h5 className="card-title">Card with stretched link</h5>
              <a href="#" className="btn btn-info mt-2 stretched-link">
                Go somewhere
              </a>
            </div>
          </div>
        </Col>
        <Col sm="6" lg="3">
          <div className="card">
            <img src={small1} className="card-img-top" alt="..." />
            <div className="card-body">
              <h5 className="card-title">
                <a href="#" className="stretched-link">
                  Card with stretched link
                </a>
              </h5>
              <p className="card-text">
                Some quick example text to build on the card up the bulk of the
                card's content.
              </p>
            </div>
          </div>
        </Col>
      </Row>
      <Row>
        <Col xs="12">
          <h4 className="mb-4">Card Group</h4>
        </Col>
      </Row>
      {/* Card Group */}
      <Row className="mb-4">
        <Col xs="12">
          <div className="card-group">
            <div className="card d-block">
              <img className="card-img-top" src={small1} alt="Card image cap" />
              <div className="card-body">
                <h5 className="card-title">Card title</h5>
                <p className="card-text">
                  This is a wider card with supporting text below as a natural
                  lead-in to additional content. This content is a little bit
                  longer.
                </p>
                <p className="card-text">
                  <small className="text-muted">Last updated 3 mins ago</small>
                </p>
              </div>
            </div>
            <div className="card d-block">
              <img className="card-img-top" src={small2} alt="Card image cap" />
              <div className="card-body">
                <h5 className="card-title">Card title</h5>
                <p className="card-text">
                  This card has supporting text below as a natural lead-in to
                  additional content.
                </p>
                <p className="card-text">
                  <small className="text-muted">Last updated 3 mins ago</small>
                </p>
              </div>
            </div>
            <div className="card d-block">
              <img className="card-img-top" src={small3} alt="Card image cap" />
              <div className="card-body">
                <h5 className="card-title">Card title</h5>
                <p className="card-text">
                  This is a wider card with supporting text below as a natural
                  lead-in to additional content. This card has even longer
                  content than the first to show that equal height action.
                </p>
                <p className="card-text">
                  <small className="text-muted">Last updated 3 mins ago</small>
                </p>
              </div>
            </div>
          </div>
        </Col>
      </Row>
    </PageLayout>
  );
};

export default Cards;
