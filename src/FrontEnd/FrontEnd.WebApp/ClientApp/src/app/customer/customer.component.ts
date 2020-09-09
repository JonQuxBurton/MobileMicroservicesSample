import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MobileToOrder } from '../models/MobileToOrder';
import { CustomersService } from '../services/customers.service';
import { Customer } from '../models/Customer';
import { MobilesService } from '../services/mobiles.service';
import { Mobile } from '../models/Mobile';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css']
})
export class CustomerComponent implements OnInit {

  selectIdustomerId: string;
  selectedCustomer: Customer;
  isOrderingMobile: boolean = false;
  isActivating: boolean = false;
  refreshedAt: Date;

  orderMobileFormGroup = new FormGroup({
    orderMobile: new FormGroup({
      phoneNumber: new FormControl('', [ Validators.required ]),
      contactPhoneNumber: new FormControl('', [Validators.required]),
      contactName: new FormControl('', [Validators.required])
    })
  });

  constructor(private route: ActivatedRoute, private customersService: CustomersService, private mobilesService: MobilesService) {
    this.route.params.subscribe(params => {
      this.selectIdustomerId = params['id'];
      this.refresh();
    });

  }

  ngOnInit(): void {
    this.customersService.mobileOrdered$.subscribe(x => {
      this.refresh();
      this.isOrderingMobile = false;
      this.orderMobileFormGroup.reset();
    });

    this.mobilesService.mobileActivated$.subscribe(x => {
      this.refresh();
    });

    this.mobilesService.mobileCeased$.subscribe(x => {
      this.refresh();
    });
  }

  get orderMobileForm() {
    let formGroup = (this.orderMobileFormGroup.controls.orderMobile) as FormGroup;
    return formGroup.controls;
  }

  refresh() {
    this.customersService.getCustomer(this.selectIdustomerId).subscribe(x => {
      this.selectedCustomer = x;
      this.refreshedAt = new Date();

      x.mobiles = x.mobiles.sort((a: any, b: any) => { return Date.parse(b.createdAt) - Date.parse(a.createdAt) });

      x.mobiles.forEach(x => {
        if (x.state == "WaitingForActivate") {
          this.mobilesService.getMobile(x.globalId).subscribe(y => {
            x.activationCode = y.orderHistory.filter(z => z.type == "Activate")[0].activationCode;
          });
        }
      });
    });
  }

  openOrderMobile() {
    this.isOrderingMobile = true;
  }

  openActivate() {
    this.isActivating = true;
  }

  onSubmit() {
    if (this.orderMobileFormGroup.invalid) {
      return;
    }

    let data = this.orderMobileFormGroup.value.orderMobile;

    let mobileToOrder = new MobileToOrder();
    mobileToOrder.phoneNumber = data.phoneNumber;
    mobileToOrder.name = data.contactName;
    mobileToOrder.contactPhoneNumber = data.contactPhoneNumber;

    this.customersService.orderMobile(this.selectedCustomer.globalId, data);
  }

  activate(mobile: Mobile) {
    this.mobilesService.activate(mobile);
  }

  cease(mobile: Mobile) {
    this.mobilesService.cease(mobile);
  }

  cancel() {
    this.isOrderingMobile = false;
  }
}
