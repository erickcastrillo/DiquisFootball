import React, { ChangeEvent } from 'react';
import { Button, Col, Row, Stack } from 'react-bootstrap';

import SearchInput from 'components/form/SearchInput';

type TableFiltersProps = {
  searchInputPlaceholder: string;
  searchQuery: string;
  setSearchQuery: React.Dispatch<React.SetStateAction<string>>;
  filteredQuery: string;
  setFilteredQuery: React.Dispatch<React.SetStateAction<string>>;
  handleFilter: () => void;
  handleClearFilters: () => void;
};

const TableFilters = (props: TableFiltersProps) => {
  const handleSearchInputField = (
    evt: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { value } = evt.target;
    props.setSearchQuery(value);
  };

  return (
    <Row>
      <Col xs={12} md={3}>
        <Stack direction="horizontal" gap={2} className="align-items-stretch">
          <SearchInput
            value={props.searchQuery}
            onChange={handleSearchInputField}
            placeholder={props.searchInputPlaceholder}
            name="search-input"
          />

          {props.searchQuery && (
            <Button variant="info" onClick={props.handleFilter}>
              Filter
            </Button>
          )}
        </Stack>
      </Col>
      <Col xs={12} md={9} />
      <Col xs={12}>
        {props.filteredQuery && (
          <div className="d-flex my-2 gap-1 fs-4">
            <p className="text-secondary">
              Showing results for <strong>"{props.filteredQuery}",</strong>
            </p>
            <p
              className="cursor-pointer hover-primary fst-italic text-secondary text-decoration-underline"
              onClick={props.handleClearFilters}
            >
              Clear?
            </p>
          </div>
        )}
      </Col>
    </Row>
  );
};

export default TableFilters;
