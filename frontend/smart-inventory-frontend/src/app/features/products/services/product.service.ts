// features/products/services/product.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Product, CreateProductRequest, UpdateProductRequest
} from '../../../shared/models/product.models';
import { PagedResult, QueryParameters } from '../../../shared/models/pagination.models';
import { environment } from '../../../../environments/environment';


@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly apiUrl = `${environment.apiUrl}/products`;

  constructor(private http: HttpClient) {}

  getAll(params: QueryParameters): Observable<PagedResult<Product>> {
    const httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize)
      .set('search', params.search ?? '');

    return this.http.get<PagedResult<Product>>(this.apiUrl, { params: httpParams });
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, request);
  }

  update(id: number, request: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
