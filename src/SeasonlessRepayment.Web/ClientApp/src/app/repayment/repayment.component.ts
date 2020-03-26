import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BaseApiResponse, Repayment, Repayments} from "../models";

@Component({
  selector: 'app-repayment',
  templateUrl: './repayment.component.html',
  styleUrls: ['./repayment.component.css']
})
export class RepaymentComponent implements OnInit {
  public repayments: Repayment[];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  ngOnInit() {
    this.http.get<BaseApiResponse<Repayments>>(this.baseUrl + 'repayment').subscribe(result => {
      if (result.success) {
        this.repayments = result.data.repayments;
        console.log(this.repayments);
      } else {
        console.error(result.error)
      }
    }, error => console.error(error));
  }

}
