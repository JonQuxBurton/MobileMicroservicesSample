import { Component, OnInit } from '@angular/core';
import { CustomersService } from '../services/customers.service';
import { Customer } from '../models/Customer';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CustomerToCreate } from '../models/CustomerToCreate';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.css']
})
export class CustomersComponent implements OnInit {

  customers: Customer[];
  isCreatingCustomer: boolean = false;
  refreshedAt: Date;

  newCustomerFormGroup = new FormGroup({
    newCustomer: new FormGroup({
      name: new FormControl('',
        [
          Validators.required
        ])
    })
  });

  constructor(private router: Router, private customersService: CustomersService) {
  }

  ngOnInit(): void {
    this.refresh();

    this.customersService.customerAdded$.subscribe(x => {
      this.refresh();
      this.isCreatingCustomer = false;
      this.newCustomerFormGroup.reset();
    });
  }

  private refresh() {
    this.customersService.getCustomers().subscribe(x => {
      this.customers = x;
      this.refreshedAt = new Date();
    });
  }

  get newCustomerForm() {
    let formGroup = (this.newCustomerFormGroup.controls.newCustomer) as FormGroup;
    return formGroup.controls;
  }

  loadCustomer(id: string) {
    this.router.navigate(['customer', id]);
  }

  openCreateCustomer() {
    this.isCreatingCustomer = true;
  }


  onSubmit() {
    if (this.newCustomerFormGroup.invalid) {
      return;
    }

    let name = this.newCustomerFormGroup.value.newCustomer.name
    let c = new CustomerToCreate();
    c.name = name;

    this.customersService.createCustomer(c);
  }

  cancel() {
    this.isCreatingCustomer = false;
  }
}
