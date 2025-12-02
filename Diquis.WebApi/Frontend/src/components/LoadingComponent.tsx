import React from 'react';
import { Spinner, Stack } from 'react-bootstrap';

interface Props {
  content?: string;
}

export default function LoadingComponent({ content = 'Loading...' }: Props) {
  return (
    <div>
      <Stack
        direction="horizontal"
        className="justify-content-center align-items-center gap-2"
      >
        <div>{content}</div>
        <Spinner />
      </Stack>
    </div>
  );
}
