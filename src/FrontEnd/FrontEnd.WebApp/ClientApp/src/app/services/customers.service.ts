import { Injectable } from '@angular/core';
import { Customer } from '../models/Customer';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  private apiBaseUrl: string = "http://localhost:5000/api/";

  constructor(private http: HttpClient) { }

  getCustomers(): Observable<Customer[]> {

    let url = `${this.apiBaseUrl}customers`;
    return this.http.get<Customer[]>(url);
  }

  getCustomer(id: string): Observable<Customer> {

    let url = `${this.apiBaseUrl}customers/${id}`;
    return this.http.get<Customer>(url);
  }
}
