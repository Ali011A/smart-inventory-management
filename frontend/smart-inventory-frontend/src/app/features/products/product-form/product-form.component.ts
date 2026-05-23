import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProductService } from '../services/product.service';
import { Product } from '../../../shared/models/product.models';

@Component({
  selector: 'app-product-form',
  standalone: false,
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent implements OnInit {
  productForm: FormGroup;
  loading = false;
  saving = false;
  errorMessage = '';
  isEditMode = false;
  productId?: number;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private snackBar: MatSnackBar
  ) {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      sku: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0.01)]],
      category: [''],
      description: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.productId = Number(id);
      this.isEditMode = true;
      this.loadProduct(this.productId);
    }
  }

  private loadProduct(id: number): void {
    this.loading = true;
    this.productService.getById(id).subscribe({
      next: product => {
        this.productForm.patchValue(product as Partial<Product>);
        this.loading = false;
      },
      error: err => {
        this.errorMessage = err.error?.message ?? 'Unable to load product.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    const value = this.productForm.value;

    if (this.isEditMode && this.productId) {
      this.productService.update(this.productId, {
        name: value.name,
        price: value.price,
        category: value.category,
        description: value.description
      }).subscribe({
        next: () => this.onSuccess('Product updated successfully'),
        error: err => this.onError(err, 'Unable to update product.')
      });
      return;
    }

    this.productService.create({
      name: value.name,
      sku: value.sku,
      price: value.price,
      category: value.category,
      description: value.description
    }).subscribe({
      next: () => this.onSuccess('Product created successfully'),
      error: err => this.onError(err, 'Unable to create product.')
    });
  }

  private onSuccess(message: string): void {
    this.saving = false;
    this.snackBar.open(message, 'Close', { duration: 3000 });
    this.router.navigate(['/products']);
  }

  private onError(error: any, fallback: string): void {
    this.saving = false;
    this.errorMessage = error.error?.message ?? fallback;
  }

  cancel(): void {
    this.router.navigate(['/products']);
  }
}
