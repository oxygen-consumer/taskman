import {Component, ViewChild} from '@angular/core';
import {ProjectService} from "../service/project-service.service";
import {Table, TableModule} from "primeng/table";
import {InputTextModule} from "primeng/inputtext";
import {FormsModule} from "@angular/forms";
import {RippleModule} from "primeng/ripple";
import {ButtonModule} from "primeng/button";
import {NgIf} from "@angular/common";
import {ProgressSpinnerModule} from "primeng/progressspinner";
import {ToastModule} from "primeng/toast";


@Component({
  selector: 'app-projects',
  standalone: true,
  templateUrl: './projects.component.html',
  imports: [
    TableModule,
    InputTextModule,
    FormsModule,
    RippleModule,
    ButtonModule,
    NgIf,
    ProgressSpinnerModule,
    ToastModule
  ],
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent {
  data: any;
  token: any;
  loading = true;
  @ViewChild('dt') table: Table;
  useTable: any;
  addRow: boolean = true;
  saveRow: string;
  clonedRows: { [s: string]: Projects } = {};
  accesToken = "acces_token";
  refreshToken = "refresh_token";

  constructor(private service: ProjectService) {
    this.token = sessionStorage.getItem(this.accesToken);
  }

  ngOnInit() {
    this.loading = true;
    this.loadElements();
  }

  loadElements() {
    this.service.getProjects(this.token).subscribe(result => {
      this.data = result;
      this.loading = false;
    })
  }

  onRowEditInit(row: any, index: any) {
    this.clonedRows[index] = {...this.data[index]};
    this.saveRow = "edit";
    this.addRow = false;

  }

  onRowEditSave(row: any, index: any) {
    this.loading = true;
    if (this.saveRow == "edit") {
      const savedObject = {
        "id": row['id'],
        "name": row['name'],
        "description": row['description']
      }
      console.log(savedObject);
      this.service.modifyProject(row["id"], savedObject).subscribe(result => {

        this.loading = false;
      }, error => {
        console.error('Error occured');
        this.loading = false;
      });


    } else {
      const savedObject = {
        "name": row['name'],
        "description": row['description']
      }
      this.service.addProject(savedObject).subscribe(result => {
        this.loadElements();
        this.loading = false;
      }, error => {
        console.error('Error occured');
        this.loading = false;

      });
    }
    this.addRow = true;
    delete this.clonedRows[index];
  }

  onRowEditCancel(row: any, rowIndex: any) {
    if (this.saveRow == "edit") {
      this.data[rowIndex] = this.clonedRows[rowIndex];
      delete this.clonedRows[rowIndex];
    } else {
      this.data.shift();
    }
    this.addRow = true;
  }

  onAddNewRow() {
    this.data.unshift({});
    this.table?.initRowEdit({});
    this.addRow = false;
    this.saveRow = "add";
  }


}

export interface Projects {
  name?: string;
  description?: string;

}
