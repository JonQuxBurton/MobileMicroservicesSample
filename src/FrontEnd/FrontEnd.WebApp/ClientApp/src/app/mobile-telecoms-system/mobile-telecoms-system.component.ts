import { Component, OnInit } from '@angular/core';
import { MobileTelecomsNetworkOrder } from "../models/MobileTelecomsNetworkOrder";

@Component({
  selector: 'app-mobile-telecoms-system',
  templateUrl: './mobile-telecoms-system.component.html',
  styleUrls: ['./mobile-telecoms-system.component.css']
})
export class MobileTelecomsSystemComponent implements OnInit {
  private orders: Array<MobileTelecomsNetworkOrder>;

  constructor() {
    this.orders = [];
    const order = new MobileTelecomsNetworkOrder();
    order.reference = "ORD001";
    order.contactPhoneNumber = "07930123123";
    order.createdAt = new Date();
    order.type = "Activate";
    order.status = "New";
    this.orders.push(order);

  }

  ngOnInit(): void {
  }

  complete(order: MobileTelecomsNetworkOrder) {
    order.status = "Completed";
  }
}
