import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { StockResponse } from '../models/stock.interface';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private cacheMap = new Map<string, Observable<StockResponse>>();

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  getStockData(symbol: string): Observable<StockResponse> {
    const cached = this.cacheMap.get(symbol);
    if (cached) return cached;

    const headers = this.getAuthHeaders();
    const request = this.http.get<StockResponse>(`${environment.apiUrl}/stock/${symbol}`, { headers })
      .pipe(shareReplay(1));

    this.cacheMap.set(symbol, request);

    // Clear cache after 5 minutes
    setTimeout(() => this.cacheMap.delete(symbol), 5 * 60 * 1000);

    return request;
  }

  getAllStocks(): Observable<StockResponse[]> {
    const headers = this.getAuthHeaders();
    return this.http.get<StockResponse[]>(`${environment.apiUrl}/stock`, { headers });
  }
}
