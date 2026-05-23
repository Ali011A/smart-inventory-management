// features/inventory/services/inventory.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  TransactionRequest, TransactionResponse, InventoryItem
} from '../../../shared/models/inventory.models';
import { PagedResult, QueryParameters } from '../../../shared/models/pagination.models';
import { environment } from '../../../../environments/environment';


@Injectable({ providedIn: 'root' })
export class InventoryService {
  private readonly apiUrl = `${environment.apiUrl}/inventory`;

  constructor(private http: HttpClient) {}

  processTransaction(request: TransactionRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/transactions`, request);
  }

  getHistory(
    params: QueryParameters,
    productId?: number,
    warehouseId?: number
  ): Observable<PagedResult<TransactionResponse>> {
    let httpParams = new HttpParams()
      .set('page', params.page)
      .set('pageSize', params.pageSize);

    if (params.search) httpParams = httpParams.set('search', params.search);
    if (productId) httpParams = httpParams.set('productId', productId);
    if (warehouseId) httpParams = httpParams.set('warehouseId', warehouseId);

    return this.http.get<PagedResult<TransactionResponse>>(
      `${this.apiUrl}/history`, { params: httpParams });
  }

  getCurrentStock(warehouseId?: number): Observable<InventoryItem[]> {
    let httpParams = new HttpParams();
    if (warehouseId) httpParams = httpParams.set('warehouseId', warehouseId);
    return this.http.get<InventoryItem[]>(`${this.apiUrl}/stock`, { params: httpParams });
  }
}
