import { Component, OnInit } from '@angular/core';
import { SimCardsOrder } from "../models/SimCardsOrder";

@Component({
  selector: 'app-sim-card-system',
  templateUrl: './sim-card-system.component.html',
  styleUrls: ['./sim-card-system.component.css']
})
export class SimCardSystemComponent implements OnInit {

  private orders: Array<SimCardsOrder>;

  constructor() {
    this.orders = [];
    const order = new SimCardsOrder();
    order.reference = "ORD001";
    order.contactPhoneNumber = "07930123123";
    order.createdAt = new Date();
    order.state = "New";
    this.orders.push(order);
  }

  ngOnInit(): void {
  }

  complete(order: SimCardsOrder) {
    order.state = "Completed";
  }
}
