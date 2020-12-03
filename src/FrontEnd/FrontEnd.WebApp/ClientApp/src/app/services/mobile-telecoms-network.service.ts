import { environment } from '../../environments/environment';
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { MobileTelecomsNetworkOrder } from "../models/MobileTelecomsNetworkOrder";
import { Observable } from "rxjs";
import { Subject } from 'rxjs';

@Injectable({
  providedIn: "root"
})
export class MobileTelecomsNetworkService {
  private orderCompletedSource = new Subject();

  orderCompletedSource$ = this.orderCompletedSource.asObservable();

  private apiBaseUrl = environment.mobileTelecomsNetworkServiceApiBaseUrl;

  constructor(private http: HttpClient) {}

  getOrders(): Observable<Array<MobileTelecomsNetworkOrder>> {
    const headers = { "Content-Type": "application/json" };
    const url = `${this.apiBaseUrl}orders`;
    return this.http.get<Array<MobileTelecomsNetworkOrder>>(url, { headers });
  }

  complete(order: MobileTelecomsNetworkOrder) {
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
