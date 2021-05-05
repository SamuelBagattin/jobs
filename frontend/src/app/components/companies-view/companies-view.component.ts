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
    this.technosFormControl.valueChanges.pipe(distinctUntilChanged()).subscribe(e => {
      if (e === "Any") {
        this.filteredCompaniesSubject.next(this.data.Companies.Companies)
      } else {
        const techno = this.data.Technologies.Technologies.find(
          techno => techno.TechnologyName == e
        );
        const filtreredCompanies = techno.CompaniesWithMainTechnologies.Ids.map(
          companyId => this.data.Companies.Companies.find(f => f.Id === companyId));

        const filteredCompaniesWithJobs = filtreredCompanies.map(e => {
            const company = Object.assign({}, e);
            company.Jobs = techno.JobsWithMainTechnology.Ids.filter(
              x => e.Jobs.map(y => y.Id).includes(x)
            ).map(
              id => {
                return e.Jobs.find(s => s.Id === id)
              }
            );
            return company
          }
        )

        this.filteredCompaniesSubject.next(
          filteredCompaniesWithJobs
        )
      }
    })
  }

}
