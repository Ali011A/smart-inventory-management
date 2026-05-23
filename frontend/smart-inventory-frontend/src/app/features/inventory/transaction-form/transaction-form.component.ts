import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { InventoryService } from '../services/inventory.service';
import { ProductService } from '../../products/services/product.service';
import { WarehouseService } from '../../warehouses/services/warehouse.service';
import { Product } from '../../../shared/models/product.models';
import { Warehouse } from '../../../shared/models/warehouse.models';
import { TransactionRequest } from '../../../shared/models/inventory.models';

@Component({
  selector: 'app-transaction-form',
  standalone: false,
  templateUrl: './transaction-form.component.html',
  styleUrls: ['./transaction-form.component.scss']
})
export class TransactionFormComponent implements OnInit {
  transactionForm: FormGroup;
  loading = false;
  saving = false;
  errorMessage = '';
  products: Product[] = [];
  warehouses: Warehouse[] = [];
  transactionTypes = ['In', 'Out'] as const;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private inventoryService: InventoryService,
    private productService: ProductService,
    private warehouseService: WarehouseService,
    private snackBar: MatSnackBar
  ) {
    this.transactionForm = this.fb.group({
      productId: [null, Validators.required],
      warehouseId: [null, Validators.required],
      type: ['In', Validators.required],
      quantity: [null, [Validators.required, Validators.min(1)]],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadWarehouses();
  }

  loadProducts(): void {
    this.productService.getAll({ page: 1, pageSize: 100 }).subscribe({
      next: result => {
        this.products = result.items;
      }
    });
  }

  loadWarehouses(): void {
    this.warehouseService.getAll({ page: 1, pageSize: 100 }).subscribe({
      next: result => {
        this.warehouses = result.items;
      }
    });
  }

  onSubmit(): void {
    if (this.transactionForm.invalid) {
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    const request = this.transactionForm.value as TransactionRequest;

    this.inventoryService.processTransaction(request).subscribe({
      next: () => {
        this.saving = false;
        this.snackBar.open('Transaction processed successfully', 'Close', { duration: 3000 });
        this.router.navigate(['/inventory']);
      },
      error: err => {
        this.saving = false;
        this.errorMessage = err.error?.message ?? 'Unable to process inventory transaction.';
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/inventory']);
  }
}
