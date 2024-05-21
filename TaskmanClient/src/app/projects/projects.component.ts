import {Component, OnInit, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {LoginServiceService} from "../service/login-service.service";
import {ProjectService} from "../service/project-service.service";
import {Table, TableModule} from "primeng/table";
import {InputTextModule} from "primeng/inputtext";
import {FormsModule} from "@angular/forms";
import {RippleModule} from "primeng/ripple";
import {ButtonModule} from "primeng/button";
import {NgIf} from "@angular/common";


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
    NgIf
  ],
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent {
  data:any;
  token:any;
  @ViewChild('dt') table: Table;
  useTable:any;
  clonedRows:{[s:string]:Projects} = {};
  accesToken = "acces_token";
  refreshToken = "refresh_token";
  constructor(private service:ProjectService){
    this.token = sessionStorage.getItem(this.accesToken);
   }

    ngOnInit(){
      console.log("buna");
      console.log(this.token);
      this.service.getProjects(this.token).subscribe(result =>{
        this.data = result;
        console.log(result);
      })
    }


  onRowEditInit(row: any,index:any) {
    this.clonedRows[index] = {...this.data[index]};

  }

  onRowEditSave(row: any,index:any) {
    //momentan gol
  }

  onRowEditCancel(row: any, rowIndex: any) {
    this.data[rowIndex] = this.clonedRows[rowIndex];
    delete this.clonedRows[rowIndex];
  }
}

export interface Projects {
  name?: string;
  description?: string;

}
