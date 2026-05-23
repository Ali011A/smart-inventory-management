import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../services/product.service';
import { Product } from '../../../shared/models/product.models';
import { QueryParameters } from '../../../shared/models/pagination.models';

@Component({
  selector: 'app-product-list',
  standalone: false,
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {
  displayedColumns = ['name', 'sku', 'category', 'price', 'actions'];
  products: Product[] = [];
  loading = false;
  errorMessage = '';
  page = 1;
  pageSize = 10;
  totalCount = 0;
  search = '';

  constructor(private productService: ProductService, private router: Router) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.errorMessage = '';

    const params: QueryParameters = {
      page: this.page,
      pageSize: this.pageSize,
      search: this.search || undefined
    };

    this.productService.getAll(params).subscribe({
      next: result => {
        this.products = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: err => {
        this.errorMessage = err.error?.message ?? 'Failed to load products.';
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.page = 1;
    this.loadProducts();
  }

  clearSearch(): void {
    this.search = '';
    this.onSearch();
  }

  createProduct(): void {
    this.router.navigate(['/products/create']);
  }

  editProduct(product: Product): void {
    this.router.navigate(['/products/edit', product.id]);
  }

  deleteProduct(product: Product): void {
    const confirmed = confirm(`Delete product "${product.name}"?`);
    if (!confirmed) {
      return;
    }

    this.loading = true;
    this.productService.delete(product.id).subscribe({
      next: () => this.loadProducts(),
      error: err => {
        this.errorMessage = err.error?.message ?? 'Failed to delete product.';
        this.loading = false;
      }
    });
  }
}
