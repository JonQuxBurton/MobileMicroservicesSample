import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { Customer } from '../models/Customer';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { CustomerToCreate } from '../models/CustomerToCreate';
import { MobileToOrder } from '../models/MobileToOrder';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  private customerAddedSource = new Subject();
  private mobileOrderedSource = new Subject();

  customerAdded$ = this.customerAddedSource.asObservable();
  mobileOrdered$ = this.mobileOrderedSource.asObservable();

  private apiBaseUrl: string = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  getCustomers(): Observable<Customer[]> {

    let url = `${this.apiBaseUrl}customers`;
    return this.http.get<Customer[]>(url);
  }

  getCustomer(id: string): Observable<Customer> {
    let url = `${this.apiBaseUrl}customers/${id}`;
    return this.http.get<Customer>(url);
  }

  createCustomer(newCustomer: CustomerToCreate) {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}customers`;
    this.http.post<any>(url, JSON.stringify(newCustomer), { headers }).subscribe({
      next: response => {
        this.customerAddedSource.next();
      },
      error: error => console.error('Error: ', error)
    });
  }

  orderMobile(customerId: string, mobileToOrder: MobileToOrder) {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}customers/${customerId}/provision`;
    this.http.post<any>(url, JSON.stringify(mobileToOrder), { headers }).subscribe({
      next: response => {
        this.mobileOrderedSource.next();
      },
      error: error => console.error('Error: ', error)
    });

  }
}
