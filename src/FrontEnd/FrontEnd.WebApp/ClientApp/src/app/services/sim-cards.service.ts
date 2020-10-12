import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivationCode } from '../models/ActivationCode';
import { SimCardsOrder } from "../models/SimCardsOrder";

@Injectable({
  providedIn: 'root'
})
export class SimCardsService {
  private apiBaseUrl: string = "http://localhost:5001/api/";

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
}
