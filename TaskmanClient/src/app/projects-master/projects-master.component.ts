﻿import {Component, OnInit, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {LoginServiceService} from "../service/login-service.service";
import {ProjectService} from "../service/project-service.service";
import {Table, TableModule} from "primeng/table";
import {InputTextModule} from "primeng/inputtext";
import {FormsModule} from "@angular/forms";
import {RippleModule} from "primeng/ripple";
import {ButtonModule} from "primeng/button";
import {NgIf} from "@angular/common";
import {ProgressSpinnerModule} from "primeng/progressspinner";
import {ToastModule} from "primeng/toast";
import {ProjectsComponent} from "./projects/projects.component";
import {ProjectsDetailComponent} from "./projects-detail/projects-detail.component";


@Component({
  selector: 'app-projects-master',
  standalone: true,
  templateUrl: './projects-master.component.html',
  imports: [
    TableModule,
    InputTextModule,
    FormsModule,
    RippleModule,
    ButtonModule,
    NgIf,
    ProgressSpinnerModule,
    ToastModule,
    ProjectsComponent,
    ProjectsDetailComponent
  ],
  styleUrl: './projects-master.component.scss'
})
export class ProjectsMasterComponent {
  row:any;
  isDetailOn:boolean = false;
  handleData($event: any){
    this.row = $event;
    this.modifyDetail();
  }

  modifyDetail(){
    this.isDetailOn = !this.isDetailOn;
  }
  ngOnInit(){

  }
}