type TableRecord = {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
};

const records: TableRecord[] = [
  { id: 1, firstName: 'Mark', lastName: 'Otto', username: '@mdo' },
  { id: 2, firstName: 'Jacob', lastName: 'Thornton', username: '@fat' },
  { id: 3, firstName: 'Dave', lastName: 'G', username: '@dave' },
  { id: 4, firstName: 'Nik', lastName: 'N', username: '@nikn' },
  { id: 5, firstName: 'Shreyu', lastName: 'Navadiya', username: '@sn' }
];

export { records };
