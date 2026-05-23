export type TransactionType = 'In' | 'Out';

export interface TransactionRequest {
  productId: number;
  warehouseId: number;
  type: TransactionType;
  quantity: number;
  notes?: string;
}

export interface TransactionResponse {
  id: number;
  productId: number;
  productName: string;
  warehouseId: number;
  warehouseName: string;
  transactionType: string;
  quantity: number;
  notes?: string;
  performedBy: string;
  createdAt: string;
}

export interface InventoryItem {
  productId: number;
  productName: string;
  sku: string;
  warehouseId: number;
  warehouseName: string;
  quantity: number;
  lastUpdated: string;
}
