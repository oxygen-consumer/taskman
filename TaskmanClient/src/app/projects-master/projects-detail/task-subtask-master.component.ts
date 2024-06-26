﻿import {Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {Table, TableModule} from "primeng/table";
import {InputTextModule} from "primeng/inputtext";
import {FormsModule} from "@angular/forms";
import {RippleModule} from "primeng/ripple";
import {ButtonModule} from "primeng/button";
import {NgIf} from "@angular/common";
import {ProgressSpinnerModule} from "primeng/progressspinner";
import {ToastModule} from "primeng/toast";
import {ProjectsComponent} from "../projects/projects.component";
import {TasksComponent} from "../projects-detail/tasks/tasks.component";
import {SubtasksComponent} from "../projects-detail/subtasks/subtasks.component";
import {TaskService} from "../../service/task-service.service";
import {LoginServiceService} from "../../service/login-service.service";
import {ProjectService} from "../../service/project-service.service";
import {Router} from "@angular/router";


@Component({
  selector: 'app-task-subtask-master',
  standalone: true,
  templateUrl: './task-subtask-master.component.html',
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
    TasksComponent,
    SubtasksComponent
  ],
  styleUrl: './task-subtask-master.component.scss'
})
export class TaskSubtaskMasterComponent {
  isDetailOnTaskSubtask:boolean = false;
  @Input() row:any;
  @ViewChild("tasks")
  tasks: any;
  @Output() emitterBack = new EventEmitter<any>();
  rol:any;
  private token: string | null;
  accesToken = "acces_token";
  refreshToken = "refresh_token";
  constructor(private service:TaskService , private loginService:LoginServiceService, private projectService:ProjectService, private router:Router){
    this.token = sessionStorage.getItem(this.accesToken);
  }
  handleData($event: any){
    this.row = $event;
    this.modifyDetailTaskSubtask();
  }


  modifyDetailTaskSubtask(){
    this.isDetailOnTaskSubtask = !this.isDetailOnTaskSubtask;
  }
  ngOnInit(){
    this.projectService.getRole(this.row.id).subscribe(result=>{;
      this.rol = result;
    },error => {
      console.error(error);
    })
  }

  onBack(){
    console.log("task-subtask-master");
    this.emitterBack.emit("back");
  }
}
