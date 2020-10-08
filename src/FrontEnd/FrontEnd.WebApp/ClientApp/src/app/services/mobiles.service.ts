import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Mobile } from '../models/Mobile';
import { Subject } from 'rxjs';
import { AvailablePhoneNumbers } from '../models/AvailablePhoneNumbers';

@Injectable({
  providedIn: 'root'
})
export class MobilesService {
  private mobileActivatedSource = new Subject();
  private mobileCeasedSource = new Subject();

  mobileActivated$ = this.mobileActivatedSource.asObservable();
  mobileCeased$ = this.mobileCeasedSource.asObservable();

  private apiBaseUrl: string = "http://localhost:5000/api/";

  constructor(private http: HttpClient) { }

  getMobile(id: string) {
    let url = `${this.apiBaseUrl}mobiles/${id}`;
    return this.http.get<Mobile>(url);
  }

  getAvailablePhoneNumber() {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}mobiles/availablePhoneNumbers`;
    return this.http.get<AvailablePhoneNumbers>(url, { headers });
  }

  getActivationCode() {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}mobiles/availablePhoneNumbers`;
    return this.http.get<AvailablePhoneNumbers>(url, { headers });
  }

  activate(mobile: Mobile) {
    let headers = { "Content-Type": 'application/json' }
    let url = `${this.apiBaseUrl}mobiles/${mobile.globalId}/activate`;
    let data = { "ActivationCode": mobile.activationCode }
    this.http.post<any>(url, JSON.stringify(data), { headers }).subscribe({
      next: response => {
        this.mobileActivatedSource .next();
      },
      error: error => console.error('Error: ', error)
    });
  }

  cease(mobile: Mobile) {
    let url = `${this.apiBaseUrl}mobiles/${mobile.globalId}`;
    this.http.delete<any>(url).subscribe({
      next: response => {
        this.mobileCeasedSource.next();
      },
      error: error => console.error('Error: ', error)
    });
  }
}
