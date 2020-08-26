import { Mobile } from "./Mobile";

export class Customer {
  public globalId: string;
  public name: string;
  public createdAt: Date;
  public updatedAt: Date;
  public mobiles: Mobile[];
}
