import React from 'react';

import SearchInput, { SearchInputProps } from 'components/form/SearchInput';
import debounce from 'lodash/debounce';

export interface GlobalFilterProps {
  globalFilter: string;
  setGlobalFilter: React.Dispatch<React.SetStateAction<string>>;
  inputProps?: SearchInputProps;
}

const GlobalFilter: React.FC<GlobalFilterProps> = ({
  globalFilter,
  setGlobalFilter,
  inputProps
}) => {
  const [value, setValue] = React.useState(globalFilter);
  const onChange = debounce((value: string) => {
    setGlobalFilter(value || '');
  }, 200); // 200 is the delay for the user input

  return (
    <SearchInput
    style={{maxWidth: "250px"}}
      value={value || ''}
      label="Search"
      onChange={(e) => {
        setValue(e.target.value);
        onChange(e.target.value);
      }}
      {...inputProps}
    />
  );
};

export default GlobalFilter;
