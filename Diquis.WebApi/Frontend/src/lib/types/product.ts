// product - sample business entity
export interface Product {
  id: string;
  name: string;
  description: string;
  createdOn: string;
  locale: Locale;
}

// create new product
export interface AddProductRequest {
  name: string;
  description: string;
  locale: Locale;
}

export enum Locale {
  En = 'En',
  Es = 'Es',
}
