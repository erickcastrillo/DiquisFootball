import React from 'react';
import { Dropdown } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import clsx from 'clsx';

type MenuItem = {
  label: string;
  icon?: string;
  variant?: string;
  hasDivider?: boolean;
};

type CardTitleProps = {
  menuItems: Array<MenuItem>;
  title: string | React.ReactNode;
  containerClass: string;
  icon?: string;
};

const CardTitle = ({
  title,
  containerClass,
  icon,
  menuItems
}: CardTitleProps) => {
  return (
    <div className={clsx(containerClass)}>
      {typeof title === 'string' ? (
        <h4 className="header-title mb-0">{title}</h4>
      ) : (
        title
      )}
      <Dropdown>
        <Dropdown.Toggle as={Link} to="" className="arrow-none card-drop">
          <i className={clsx(icon ? icon : 'mdi mdi-dots-vertical')} />
        </Dropdown.Toggle>
        <Dropdown.Menu align="end">
          {(menuItems || []).map((item, index) => {
            return (
              <React.Fragment key={index.toString()}>
                {item.hasDivider && <Dropdown.Divider as="div" />}
                <Dropdown.Item
                  className={clsx(item.variant ? item.variant : '')}
                >
                  {item.icon && <i className={clsx(item.icon, 'me-1')}></i>}
                  {item.label}
                </Dropdown.Item>
              </React.Fragment>
            );
          })}
        </Dropdown.Menu>
      </Dropdown>
    </div>
  );
};

export default CardTitle;
