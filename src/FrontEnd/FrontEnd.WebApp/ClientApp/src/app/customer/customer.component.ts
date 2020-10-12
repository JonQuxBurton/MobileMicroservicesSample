import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MobileToOrder } from '../models/MobileToOrder';
import { CustomersService } from '../services/customers.service';
import { Customer } from '../models/Customer';
import { MobilesService } from '../services/mobiles.service';
import { Mobile } from '../models/Mobile';
import { ActivatedRoute } from '@angular/router';
import { SimCardsService } from '../services/sim-cards.service';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css']
})
export class CustomerComponent implements OnInit {
  chosenPhoneNumber: string;
  selectIdustomerId: string;
  selectedCustomer: Customer;
  isOrderingMobile: boolean = false;
  isActivating: boolean = false;
  refreshedAt: Date;

  orderMobileFormGroup = new FormGroup({
    orderMobile: new FormGroup({
      contactPhoneNumber: new FormControl('', [Validators.required]),
      contactName: new FormControl('', [Validators.required])
    })
  });

  constructor(private route: ActivatedRoute, private customersService: CustomersService, private mobilesService: MobilesService, private simCardsService: SimCardsService) {
    this.route.params.subscribe(params => {
      this.selectIdustomerId = params['id'];
      this.refresh();
    });

  }

  ngOnInit(): void {
    this.mobilesService.getAvailablePhoneNumber().subscribe(x => {
      this.chosenPhoneNumber = x.phoneNumbers[0];
    })

    this.customersService.mobileOrdered$.subscribe(x => {
      this.refresh();
      this.isOrderingMobile = false;
      this.orderMobileFormGroup.reset();
      this.mobilesService.getAvailablePhoneNumber().subscribe(x => {
        this.chosenPhoneNumber = x.phoneNumbers[0];
      })
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

      this.refreshActivationCodes();
    });

  }

  private activationCodes: { [key: string]: string; } = {};

  refreshActivationCodes() {
    this.selectedCustomer.mobiles.filter(x => x.state == "WaitingForActivate").forEach(y => {
      let lastOrder = y.orderHistory[y.orderHistory.length - 1];

      if (lastOrder) {
        this.simCardsService.getActivationCode(lastOrder.globalId).subscribe(z => {
          this.activationCodes[y.phoneNumber] = z.activationCode;
      });
      }
    });
  }

  getActivationCode(mobile: Mobile): string {
    return this.activationCodes[mobile.phoneNumber];
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
    mobileToOrder.phoneNumber = this.chosenPhoneNumber;
    mobileToOrder.name = data.contactName;
    mobileToOrder.contactPhoneNumber = data.contactPhoneNumber;

    this.customersService.orderMobile(this.selectedCustomer.globalId, mobileToOrder);
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

  getActionState(mobile: Mobile): string {
    if (mobile.state == "WaitingForActivate" ||
        mobile.state == "Live" ||
        mobile.state == "Suspended") {
      return mobile.state;
    }

    return "OrderInProgress"
  }

  getOrderInProgress(mobile: Mobile) {
    if (mobile.orderHistory.length == 0)
      return "";

    let lastOrder = mobile.orderHistory[mobile.orderHistory.length - 1];
    return `'${lastOrder.type}' order '${lastOrder.state}'`;
  }
}
