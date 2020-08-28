import { Component, OnInit } from '@angular/core';
import { StageControllerService } from '../services/stage-controller.service';
import { FormGroup, FormControl } from '@angular/forms';
import { MobileToOrder } from '../models/MobileToOrder';
import { CustomersService } from '../services/customers.service';
import { Customer } from '../models/Customer';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css']
})
export class CustomerComponent implements OnInit {

  selectedCustomer: Customer;
  isOrderingMobile: boolean = false;

  orderMobileFormGroup = new FormGroup({
    orderMobile: new FormGroup({
      phoneNumber: new FormControl(''),
      contactPhoneNumber: new FormControl(''),
      name: new FormControl('')
    })
  });

  constructor(private stageController: StageControllerService, private customersService: CustomersService) { }

  ngOnInit(): void {
    this.customersService.getCustomer(this.stageController.selectedCustomerId).subscribe(x => {
      this.selectedCustomer = x;
    });

    this.customersService.mobileOrdered$.subscribe(x => {
      this.refresh();
      this.isOrderingMobile = false;
      this.orderMobileFormGroup.reset();
    });

  }

  refresh() {
    this.customersService.getCustomer(this.stageController.selectedCustomerId).subscribe(x => {
      this.selectedCustomer = x;
    });
  }

  back() {
    this.stageController.toCustomers();
  }

  openOrderMobile() {
    this.isOrderingMobile = true;
  }


  onSubmit() {
    let data = this.orderMobileFormGroup.value.orderMobile;

    let mobileToOrder = new MobileToOrder();
    mobileToOrder.phoneNumber = data.phoneNumber;
    mobileToOrder.name = data.name;
    mobileToOrder.contactPhoneNumber = data.contactPhoneNumber;

    this.customersService.orderMobile(this.selectedCustomer.globalId, data);
  }

  cancel() {
    this.isOrderingMobile = false;
  }
}
