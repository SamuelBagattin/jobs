import {Component, Input, OnInit} from '@angular/core';
import {Company, RootObject} from "../../models/company";
import {BehaviorSubject, Observable} from "rxjs";
import {FormControl} from "@angular/forms";
import {debounceTime, distinctUntilChanged} from "rxjs/operators";

@Component({
  selector: 'app-companies-view',
  templateUrl: './companies-view.component.html',
  styleUrls: ['./companies-view.component.scss']
})
export class CompaniesViewComponent implements OnInit {

  @Input() public data: RootObject = {
    Companies: {
      Companies: [],
      Count: 0
    },
    Technologies: {
      Technologies: [],
      TechnologiesCount: 0
    }
  }
  public filteredCompaniesSubject: BehaviorSubject<Company[]> = new BehaviorSubject<Company[]>([])
  public filteredCompanies$: Observable<Company[]> = this.filteredCompaniesSubject.asObservable()

  public technosFormControl: FormControl = new FormControl("Any")

  constructor() {
  }

  ngOnInit(): void {
    this.filteredCompaniesSubject.next(this.data.Companies.Companies);
    this.technosFormControl.valueChanges.pipe(distinctUntilChanged(), debounceTime(200)).subscribe(e => {
      if (e === "Any") {
        this.filteredCompaniesSubject.next(this.data.Companies.Companies)
      } else {
        this.filteredCompaniesSubject.next(this.data.Technologies.Technologies.find(techno => techno.TechnologyName == e).CompaniesWithMainTechnologies.Ids.map(companyId => this.data.Companies.Companies.find(f => f.Id === companyId)))
      }
    })
  }

}
