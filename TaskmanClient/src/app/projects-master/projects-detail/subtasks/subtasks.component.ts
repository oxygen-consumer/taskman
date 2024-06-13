import {Component, EventEmitter, OnInit, Output, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {LoginServiceService} from "../../../service/login-service.service";
import {ProjectService} from "../../../service/project-service.service";
import {Table, TableModule} from "primeng/table";
import {InputTextModule} from "primeng/inputtext";
import {FormsModule} from "@angular/forms";
import {RippleModule} from "primeng/ripple";
import {ButtonModule} from "primeng/button";
import {NgIf} from "@angular/common";
import {ProgressSpinnerModule} from "primeng/progressspinner";
import {ToastModule} from "primeng/toast";
import {ProjectsComponent} from "../../projects/projects.component";
import {HeaderComponent} from "../../../header/header.component";
import {NavigationComponent} from "../../../navigation/navigation.component";
import {DashboardComponent} from "../../../dashboard/dashboard.component";


@Component({
  selector: 'app-subtasks',
  standalone: true,
  templateUrl: './subtasks.component.html',
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
    HeaderComponent,
    NavigationComponent,
    DashboardComponent
  ],
  styleUrl: './subtasks.component.scss'
})
export class SubtasksComponent {
  row:any;
  isDetailOnTaskSubtask:boolean = false;
  @Output() emitter = new EventEmitter<any>();

  modifyDetailTaskSubtask(){
    this.isDetailOnTaskSubtask = !this.isDetailOnTaskSubtask;
  }

}
