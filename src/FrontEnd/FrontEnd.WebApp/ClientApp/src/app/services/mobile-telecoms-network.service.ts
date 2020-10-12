import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { MobileTelecomsNetworkOrder } from "../models/MobileTelecomsNetworkOrder";
import { Observable } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class MobileTelecomsNetworkService {
  private apiBaseUrl = "http://localhost:5002/api/";

  constructor(private http: HttpClient) {}

  getOrders(): Observable<Array<MobileTelecomsNetworkOrder>> {
    const headers = { "Content-Type": "application/json" };
    const url = `${this.apiBaseUrl}orders`;
    return this.http.get<Array<MobileTelecomsNetworkOrder>>(url, { headers });
  }
}
