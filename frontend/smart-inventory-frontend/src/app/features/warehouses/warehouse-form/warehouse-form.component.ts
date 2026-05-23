import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { WarehouseService } from '../services/warehouse.service';
import { Warehouse } from '../../../shared/models/warehouse.models';

@Component({
  selector: 'app-warehouse-form',
  standalone: false,
  templateUrl: './warehouse-form.component.html',
  styleUrls: ['./warehouse-form.component.scss']
})
export class WarehouseFormComponent implements OnInit {
  warehouseForm: FormGroup;
  loading = false;
  saving = false;
  errorMessage = '';
  isEditMode = false;
  warehouseId?: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private warehouseService: WarehouseService,
    private snackBar: MatSnackBar
  ) {
    this.warehouseForm = this.fb.group({
      name: ['', Validators.required],
      location: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.warehouseId = Number(id);
      this.isEditMode = true;
      this.loadWarehouse(this.warehouseId);
    }
  }

  private loadWarehouse(id: number): void {
    this.loading = true;
    this.warehouseService.getById(id).subscribe({
      next: warehouse => {
        this.warehouseForm.patchValue(warehouse as Partial<Warehouse>);
        this.loading = false;
      },
      error: err => {
        this.errorMessage = err.error?.message ?? 'Unable to load warehouse.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.warehouseForm.invalid) {
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    if (this.isEditMode && this.warehouseId) {
      this.warehouseService.update(this.warehouseId, this.warehouseForm.value).subscribe({
        next: () => this.onSuccess('Warehouse updated successfully'),
        error: err => this.onError(err, 'Unable to update warehouse.')
      });
      return;
    }

    this.warehouseService.create(this.warehouseForm.value).subscribe({
      next: () => this.onSuccess('Warehouse created successfully'),
      error: err => this.onError(err, 'Unable to create warehouse.')
    });
  }

  private onSuccess(message: string): void {
    this.saving = false;
    this.snackBar.open(message, 'Close', { duration: 3000 });
    this.router.navigate(['/warehouses']);
  }

  private onError(error: any, fallback: string): void {
    this.saving = false;
    this.errorMessage = error.error?.message ?? fallback;
  }

  cancel(): void {
    this.router.navigate(['/warehouses']);
  }
}
