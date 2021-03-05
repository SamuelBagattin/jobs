import {ChangeDetectorRef, Component, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { RootObject} from "./models/company";
import {Observable, of} from "rxjs";
import {map} from "rxjs/operators";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public companies$: Observable<RootObject> = of()
  public isDataLoading: boolean = true

  constructor(private readonly httpClient: HttpClient, private readonly changeDetectorRef: ChangeDetectorRef) {

  }

  ngOnInit(): void {
    this.companies$ = this.httpClient.get<RootObject>("https://jobs.samuelbagattin.com/index.json").pipe(map(e => {
      this.isDataLoading = false;
      this.changeDetectorRef.markForCheck();
      return e
    }))
  }


}
