export class Mobile {
  public globalId: string;
  public createdAt: Date;
  public customerId: string;
  public phoneNumber: string;
  public state: string;
  public orderHistory: Order[]
  public activationCode: string;
}

export class Order {
  public globalId: string;
  public name: string;
  public contactPhoneNumber: string;
  public state: string;
  public type: string;
  public createdAt: Date;
  public activationCode: string;
}
