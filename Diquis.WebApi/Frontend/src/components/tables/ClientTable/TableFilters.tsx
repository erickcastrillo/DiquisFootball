import React from 'react';
import { Col, Row } from 'react-bootstrap';

import GlobalFilter, {
  GlobalFilterProps
} from 'components/tables/ClientTable/GlobalFilter';

type TableFiltersProps = GlobalFilterProps;

const TableFilters = (props: TableFiltersProps) => {
  return (
    <Row>
      <Col xs={12} md={3} className="mb-2">
        <GlobalFilter {...props} />
      </Col>
    </Row>
  );
};

export default TableFilters;
