import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivationCode } from '../models/ActivationCode';
import { SimCardsOrder } from "../models/SimCardsOrder";
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SimCardsService {
  private orderCompletedSource = new Subject();

  orderCompletedSource$ = this.orderCompletedSource.asObservable();

  private apiBaseUrl: string = environment.simCardsServiceApiBaseUrl;

  constructor(private http: HttpClient) { }

  getActivationCode(orderId: string): Observable<ActivationCode> {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}orders/${orderId}/activationCode`;
    return this.http.get<ActivationCode>(url, { headers });
  }

  getOrders(): Observable<Array<SimCardsOrder>> {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}orders`;
    return this.http.get<SimCardsOrder[]>(url, { headers });
  }

  complete(order: SimCardsOrder) {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}orders/${order.reference}/complete`;
    return this.http.post<any>(url, { headers }).subscribe({
      next: response => {
        this.orderCompletedSource.next();
      },
      error: error => console.error('Error: ', error)
    });
  }
}
