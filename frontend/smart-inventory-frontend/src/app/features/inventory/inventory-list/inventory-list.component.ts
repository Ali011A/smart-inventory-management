import { Component, OnInit } from '@angular/core';
import { InventoryService } from '../services/inventory.service';
import { InventoryItem } from '../../../shared/models/inventory.models';
import { WarehouseService } from '../../warehouses/services/warehouse.service';
import { Warehouse } from '../../../shared/models/warehouse.models';

@Component({
  selector: 'app-inventory-list',
  standalone: false,
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.scss']
})
export class InventoryListComponent implements OnInit {
  stock: InventoryItem[] = [];
  warehouses: Warehouse[] = [];
  selectedWarehouseId?: number;
  loading = false;
  errorMessage = '';

  constructor(
    private inventoryService: InventoryService,
    private warehouseService: WarehouseService
  ) {}

  ngOnInit(): void {
    this.loadWarehouses();
    this.loadStock();
  }

  loadWarehouses(): void {
    this.warehouseService.getAll({ page: 1, pageSize: 100 }).subscribe({
      next: result => {
        this.warehouses = result.items;
      }
    });
  }

  loadStock(): void {
    this.loading = true;
    this.errorMessage = '';

    this.inventoryService.getCurrentStock(this.selectedWarehouseId).subscribe({
      next: stock => {
        this.stock = stock;
        this.loading = false;
      },
      error: err => {
        this.errorMessage = err.error?.message ?? 'Unable to load inventory.';
        this.loading = false;
      }
    });
  }

  onWarehouseChange(): void {
    this.loadStock();
  }
}
