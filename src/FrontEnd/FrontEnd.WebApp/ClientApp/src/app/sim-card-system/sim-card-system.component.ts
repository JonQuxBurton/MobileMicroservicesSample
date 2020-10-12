import { Component, OnInit } from "@angular/core";
import { SimCardsOrder } from "../models/SimCardsOrder";
import { SimCardsService } from "../services/sim-cards.service";

@Component({
  selector: "app-sim-card-system",
  templateUrl: "./sim-card-system.component.html",
  styleUrls: ["./sim-card-system.component.css"]
})
export class SimCardSystemComponent implements OnInit {

  private orders: Array<SimCardsOrder>;
  private refreshedAt: Date;

  constructor(private simCardsService: SimCardsService) {
  }

  ngOnInit(): void {
    this.refresh();

    this.simCardsService.orderCompletedSource$.subscribe(x => {
      this.refresh();
    });
  }

  complete(order: SimCardsOrder) {
    this.simCardsService.complete(order);
  }

  private refresh() {
    this.simCardsService.getOrders().subscribe(x => {
      this.orders = x;
      this.refreshedAt = new Date();
    });
  }
}
