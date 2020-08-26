import { Component, OnInit } from '@angular/core';
import { CustomersService } from '../services/customers.service';
import { Customer } from '../models/Customer';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.css']
})
export class CustomersComponent implements OnInit {

  customers: Customer[];
  selectedCustomer: Customer;

  constructor(private customersService: CustomersService) { }

  ngOnInit(): void {
    this.customersService.getCustomers().subscribe(x => {
      this.customers = x;
    });
  }

  loadCustomer(id: string) {
    this.customersService.getCustomer(id).subscribe(x => {
      this.selectedCustomer  = x;
    });
  }
}
