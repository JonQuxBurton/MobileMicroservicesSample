import { Order } from "./Order";

export class Mobile {
  public globalId: string;
  public createdAt: Date;
  public customerId: string;
  public phoneNumber: string;
  public state: string;
  public orders: Order[];
  public activationCode: string;
}
