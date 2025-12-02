// product - sample business entity
export interface Product {
  id: string;
  name: string;
  description: string;
  createdOn: string;
}

// create new product
export interface AddProductRequest {
  name: string;
  description: string;
}
