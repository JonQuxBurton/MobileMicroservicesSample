import { Injectable } from '@angular/core';
import { CustomersService } from './customers.service';

@Injectable({
  providedIn: 'root'
})
export class StageControllerService {

  isModeCustomers: boolean = true;
  isModeCustomerLoaded: boolean;

  selectedCustomerId: string;

  constructor() { }

  toCustomers() {
    this.isModeCustomers = true;
    this.isModeCustomerLoaded = false;
  }

  toCustomerLoaded(id: string) {
    this.selectedCustomerId = id;
    this.isModeCustomers = false;
    this.isModeCustomerLoaded = true;
  }
}
