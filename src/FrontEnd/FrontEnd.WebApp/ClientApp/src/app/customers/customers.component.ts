import { Component, OnInit } from '@angular/core';
import { CustomersService } from '../services/customers.service';
import { Customer } from '../models/Customer';
import { FormControl, FormGroup } from '@angular/forms';
import { CustomerToCreate } from '../models/CustomerToCreate';
import { StageControllerService } from '../services/stage-controller.service';

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
      name: new FormControl('')
    })
  });

  constructor(private customersService: CustomersService, private stageController: StageControllerService) { }

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

  loadCustomer(id: string) {
    this.stageController.toCustomerLoaded(id);
  }

  openCreateCustomer() {
    this.isCreatingCustomer = true;
  }

  onSubmit() {
    let name = this.newCustomerFormGroup.value.newCustomer.name
    let c = new CustomerToCreate();
    c.name = name;

    this.customersService.createCustomer(c);
  }

  cancel() {
    this.isCreatingCustomer = false;
  }
}
