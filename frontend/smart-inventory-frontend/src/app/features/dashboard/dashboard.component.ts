import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ProductService } from '../products/services/product.service';
import { WarehouseService } from '../warehouses/services/warehouse.service';
import { InventoryService } from '../inventory/services/inventory.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  loading = false;
  errorMessage = '';
  totals = {
    products: 0,
    warehouses: 0,
    stockItems: 0
  };

  constructor(
    private productService: ProductService,
    private warehouseService: WarehouseService,
    private inventoryService: InventoryService
  ) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  loadSummary(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      products: this.productService.getAll({ page: 1, pageSize: 1 }),
      warehouses: this.warehouseService.getAll({ page: 1, pageSize: 1 }),
      stockItems: this.inventoryService.getCurrentStock()
    }).subscribe({
      next: result => {
        this.totals.products = result.products.totalCount;
        this.totals.warehouses = result.warehouses.totalCount;
        this.totals.stockItems = result.stockItems.length;
        this.loading = false;
      },
      error: err => {
        this.errorMessage = err.error?.message ?? 'Unable to load dashboard summary.';
        this.loading = false;
      }
    });
  }
}
