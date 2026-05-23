import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Warehouse, CreateWarehouseRequest, UpdateWarehouseRequest } from '../../../shared/models/warehouse.models';
import { PagedResult, QueryParameters } from '../../../shared/models/pagination.models';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class WarehouseService {
  private readonly apiUrl = `${environment.apiUrl}/warehouses`;

  constructor(private http: HttpClient) {}

  getAll(params: QueryParameters): Observable<PagedResult<Warehouse>> {
    let httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize);

    if (params.search) {
      httpParams = httpParams.set('search', params.search);
    }

    return this.http.get<PagedResult<Warehouse> | Warehouse[]>(this.apiUrl, { params: httpParams }).pipe(
      map(result => Array.isArray(result)
        ? {
            items: result,
            totalCount: result.length,
            page: params.page,
            pageSize: params.pageSize,
            totalPages: 1,
            hasNextPage: false,
            hasPreviousPage: false
          }
        : result)
    );
  }

  getById(id: number): Observable<Warehouse> {
    return this.http.get<Warehouse>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateWarehouseRequest): Observable<Warehouse> {
    return this.http.post<Warehouse>(this.apiUrl, request);
  }

  update(id: number, request: UpdateWarehouseRequest): Observable<Warehouse> {
    return this.http.put<Warehouse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
