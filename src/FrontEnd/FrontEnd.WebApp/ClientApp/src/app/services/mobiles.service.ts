import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class MobilesService {

  private apiBaseUrl: string = "http://localhost:5000/api/";

  constructor(private http: HttpClient) { }

  
}
