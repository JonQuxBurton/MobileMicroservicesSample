import { Component, OnInit } from '@angular/core';
import { StageControllerService } from '../services/stage-controller.service';
import { FormGroup, FormControl } from '@angular/forms';
import { MobileToOrder } from '../models/MobileToOrder';
import { CustomersService } from '../services/customers.service';
import { Customer } from '../models/Customer';
import { MobilesService } from '../services/mobiles.service';
import { Mobile } from '../models/Mobile';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css']
})
export class CustomerComponent implements OnInit {

  selectedCustomer: Customer;
  isOrderingMobile: boolean = false;
  isActivating: boolean = false;

  orderMobileFormGroup = new FormGroup({
    orderMobile: new FormGroup({
      phoneNumber: new FormControl(''),
      contactPhoneNumber: new FormControl(''),
      name: new FormControl('')
    })
  });

  constructor(private stageController: StageControllerService, private customersService: CustomersService, private mobilesService: MobilesService) { }

  ngOnInit(): void {
    this.customersService.getCustomer(this.stageController.selectedCustomerId).subscribe(x => {
      this.selectedCustomer = x;

      x.mobiles.forEach(x => {
        if (x.state == "WaitingForActivate") {
          this.mobilesService.getMobile(x.globalId).subscribe(y => {
            x.activationCode = y.orderHistory.filter(z => z.type == "Activate")[0].activationCode;
          });
        }
      });
    });

    this.customersService.mobileOrdered$.subscribe(x => {
      this.refresh();
      this.isOrderingMobile = false;
      this.orderMobileFormGroup.reset();
    });

    this.mobilesService.mobileActivated$.subscribe(x => {
      this.refresh();

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

  openActivate() {
    this.isActivating = true;
  }

  onSubmit() {
    let data = this.orderMobileFormGroup.value.orderMobile;

    let mobileToOrder = new MobileToOrder();
    mobileToOrder.phoneNumber = data.phoneNumber;
    mobileToOrder.name = data.name;
    mobileToOrder.contactPhoneNumber = data.contactPhoneNumber;

    this.customersService.orderMobile(this.selectedCustomer.globalId, data);
  }

  activate(mobile: Mobile) {
    this.mobilesService.activate(mobile);
  }

  cancel() {
    this.isOrderingMobile = false;
  }
}
