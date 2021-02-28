import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { RootObject} from "./models/company";
import {Observable, of} from "rxjs";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public companies$: Observable<RootObject> = of()

  constructor(private readonly httpClient: HttpClient) {

  }

  ngOnInit(): void {
    this.companies$ = this.httpClient.get<RootObject>("https://jobs.samuelbagattin.com/index.json");
  }


}
