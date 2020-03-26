import {Customer} from "./customer";
import {Season} from "./season";

export interface Repayment {
  repaymentId: Number;
  customer: Customer;
  season: Season;
  date: Date;
  amount: Number;
  parentId: Number;
}
