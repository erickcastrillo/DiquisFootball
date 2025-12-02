import React from "react";

import { Link } from "react-router-dom";
import { Card } from "react-bootstrap";

import LogoLight from "assets/images/logo.png";
import LogoDark from "assets/images/logo-dark.png";
import SmallLogo from "assets/images/logo-lg.png";
import ReactLogo from "assets/images/react.png";
import useAccountLayout from "./hooks/useAccountLayout";

type AccountLayoutProps = {
  bottomLinks?: React.ReactNode;
  children?: React.ReactNode;
};

const AccountLayout = ({ bottomLinks, children }: AccountLayoutProps) => {
  useAccountLayout();

  return (
    <div className="auth-fluid">
      {/* Auth fluid left content */}
      <div className="auth-fluid-form-box">
        <div className="align-items-center d-flex h-100">
          <Card.Body className="d-flex flex-column h-100 gap-3 p-0">
            {/* logo */}
            <div className="auth-brand text-center text-lg-start">
              <Link to="/" className="logo-dark">
                <span>
                  <img src={LogoDark} alt="" height="43" />
                </span>
              </Link>
              <Link to="/" className="logo-light">
                <span>
                  <img src={LogoLight} alt="" height="43" />
                </span>
              </Link>
            </div>

            {children}

            {/* footer links */}
            {bottomLinks}
          </Card.Body>
        </div>
      </div>

      {/* Auth fluid right content */}
      <div className="auth-fluid-right text-center">
        <div className="auth-fluid-right__logo">
          <img src={SmallLogo} alt="Small logo" />
        </div>
        <div className="auth-user-testimonial">
          <img src={ReactLogo} className="mb-2" />
          <h4 className="mb-1">{"Nano ASP.NET Boilerplate"}</h4>
          <p className="mb-3">{"React UI"}</p>
        </div>
      </div>
    </div>
  );
};

export default AccountLayout;
