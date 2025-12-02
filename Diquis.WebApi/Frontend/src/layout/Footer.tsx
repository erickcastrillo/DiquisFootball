import { Row, Col } from 'react-bootstrap';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="footer">
      <div className="container-fluid">
        <Row>
          <Col md={6}>
            {currentYear} © Nano Boilerplate -{' '}
            <a href="https://aspnano.com/" target="”_blank”">
              AspNano.com
            </a>
          </Col>

          <Col md={6}>
            <div className="text-md-end footer-links d-none d-md-block">
            <span className="badge bg-info" style={{fontSize: "16px", margin: "0 15px"}}>React UI</span>

              <a href="https://aspnano.com/guide/" target="_blank">
                Documentation
              </a>
            </div>
          </Col>
        </Row>
      </div>
    </footer>
  );
};

export default Footer;
