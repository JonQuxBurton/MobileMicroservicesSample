import { Injectable } from '@angular/core';
import { Customer } from '../models/Customer';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  constructor() { }

  getCustomers() {

    let c1 = new Customer();
    c1.name = "Armstrong Ltd";
    c1.globalId = "AAA-001"

    let c2 = new Customer();
    c2.name = "Aldrin Ltd";
    c2.globalId = "BBB-002"

    let c3 = new Customer();
    c3.name = "Collins Ltd";
    c3.globalId = "CCC-003";

    return [c1, c2, c3]
  }
}
