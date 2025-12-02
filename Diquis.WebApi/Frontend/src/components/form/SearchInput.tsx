import FormInput, { FormInputProps } from 'components/form/FormInput';

export type SearchInputProps = Omit<FormInputProps, 'name'> & {
  name?: string;
};

const SearchInput = (props: SearchInputProps) => {
  return <FormInput name="" {...props} />;
};

export default SearchInput;
