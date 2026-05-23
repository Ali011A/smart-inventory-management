export interface Warehouse {
  id: number;
  name: string;
  location?: string;
  createdAt: string;
}

export interface CreateWarehouseRequest {
  name: string;
  location?: string;
}

export interface UpdateWarehouseRequest {
  name: string;
  location?: string;
}
