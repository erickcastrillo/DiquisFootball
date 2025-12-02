import { Placement } from 'react-bootstrap/esm/types';

/* Accordion types */
export type SimpleCardAccordionCustomToggleProps = {
  eventKey: string;
  children: React.ReactNode;
};

export type AccordionItem = {
  id: number;
  title: string;
  text: string;
};

export type CustomToggleProps = {
  children: React.ReactNode;
  eventKey: string;
  containerClass: string;
  linkClass: string;
  callback?: (eventKey: string) => void;
};

export type CustomAccordionProps = {
  item: AccordionItem;
  index: number;
};

export type PopoverDirection = {
  placement: Placement;
};
