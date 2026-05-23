export interface Product {
  id: number;
  name: string;
  sku: string;
  description?: string;
  price: number;
  category?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductRequest {
  name: string;
  sku: string;
  description?: string;
  price: number;
  category?: string;
}

export interface UpdateProductRequest {
  name: string;
  description?: string;
  price: number;
  category?: string;
}
