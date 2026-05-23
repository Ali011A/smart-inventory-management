import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { WarehouseService } from '../services/warehouse.service';
import { Warehouse } from '../../../shared/models/warehouse.models';
import { QueryParameters } from '../../../shared/models/pagination.models';

@Component({
  selector: 'app-warehouse-list',
  standalone: false,
  templateUrl: './warehouse-list.component.html',
  styleUrls: ['./warehouse-list.component.scss']
})
export class WarehouseListComponent implements OnInit {
  warehouses: Warehouse[] = [];
  loading = false;
  errorMessage = '';
  search = '';
  page = 1;
  pageSize = 10;
  totalCount = 0;

  constructor(private warehouseService: WarehouseService, private router: Router) {}

  ngOnInit(): void {
    this.loadWarehouses();
  }

  loadWarehouses(): void {
    this.loading = true;
    this.errorMessage = '';

    const params: QueryParameters = {
      page: this.page,
      pageSize: this.pageSize,
      search: this.search || undefined
    };

    this.warehouseService.getAll(params).subscribe({
      next: result => {
        this.warehouses = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: err => {
        this.errorMessage = err.error?.message ?? 'Unable to load warehouses.';
        this.loading = false;
      }
    });
  }

  searchWarehouses(): void {
    this.page = 1;
    this.loadWarehouses();
  }

  clearSearch(): void {
    this.search = '';
    this.searchWarehouses();
  }

  createWarehouse(): void {
    this.router.navigate(['/warehouses/create']);
  }

  editWarehouse(warehouse: Warehouse): void {
    this.router.navigate(['/warehouses/edit', warehouse.id]);
  }

  deleteWarehouse(warehouse: Warehouse): void {
    if (!confirm(`Delete warehouse "${warehouse.name}"?`)) {
      return;
    }
    this.loading = true;
    this.warehouseService.delete(warehouse.id).subscribe({
      next: () => this.loadWarehouses(),
      error: err => {
        this.errorMessage = err.error?.message ?? 'Unable to delete warehouse.';
        this.loading = false;
      }
    });
  }
}
