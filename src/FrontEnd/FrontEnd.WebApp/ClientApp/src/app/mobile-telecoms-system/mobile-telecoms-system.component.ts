import { Component, OnInit } from '@angular/core';
import { MobileTelecomsNetworkOrder } from "../models/MobileTelecomsNetworkOrder";
import { MobileTelecomsNetworkService } from "../services/mobile-telecoms-network.service";

@Component({
  selector: 'app-mobile-telecoms-system',
  templateUrl: './mobile-telecoms-system.component.html',
  styleUrls: ['./mobile-telecoms-system.component.css']
})
export class MobileTelecomsSystemComponent implements OnInit {
  private orders: Array<MobileTelecomsNetworkOrder>;
  private refreshedAt: Date;

  constructor(private mobileTelecomsNetworkService: MobileTelecomsNetworkService) {
  }

  ngOnInit(): void {
    this.refresh();

    this.mobileTelecomsNetworkService.orderCompletedSource$.subscribe(x => {
      this.refresh();
    });
  }

  complete(order: MobileTelecomsNetworkOrder) {
    this.mobileTelecomsNetworkService.complete(order);
  }

  private refresh() {
    this.mobileTelecomsNetworkService.getOrders().subscribe(x => {
      this.orders = x;
      this.refreshedAt = new Date();
    });
  }
}
