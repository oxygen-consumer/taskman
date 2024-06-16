import {Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChild, ViewChildren} from '@angular/core';
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
import {CalendarModule} from "primeng/calendar";
import {Projects} from "../tasks/tasks.component";
import {SubtaskService} from "../../../service/subtask-service";
import {TaskService} from "../../../service/task-service.service";


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
    DashboardComponent,
    CalendarModule
  ],
  styleUrl: './subtasks.component.scss'
})
export class SubtasksComponent {
  row:any;
  isDetailOnTaskSubtask:boolean = false;
  @Output() emitter = new EventEmitter<any>();
  @ViewChild('dt') table: Table;
  data:any;
  token:any;
  loading = true;
  cloneRow:any;
  @Input() task:any;
  idProject: any;
  idTask:any;
  useTable:any;
  addRow:boolean = true;
  saveRow: string;
  clonedRows:{[s:string]:Projects} = {};
  accesToken = "acces_token";
  refreshToken = "refresh_token";
  editable:boolean = true;
  editProject:boolean = false;
  email:string|null;
  userEmail: string|null;
  @Input()
  rol:string;
  modifyDetailTaskSubtask(){
    this.isDetailOnTaskSubtask = !this.isDetailOnTaskSubtask;
  }
  constructor(private service:SubtaskService , private loginService:LoginServiceService, private projectService:ProjectService,private taskService:TaskService){
    this.token = sessionStorage.getItem(this.accesToken);
  }
  onButtonClick() {
    this.emitter.emit();
  }

  ngOnInit(){
    console.log(this.task);
    this.loading = true;
    this.idProject = this.task.projectId;
    this.idTask = this.task.id;
    console.log("Acesta este task-ul",this.idTask);
    this.email = this.loginService.getEmail();
    this.loadElements();
  }

  initEditTask(){
    this.editProject = true;
    this.cloneRow = {...this.row};
  }

  editCancel(){
    this.row = this.cloneRow;
    this.editProject = false;
    delete this.cloneRow;
  }
  saveTask(){
    const savedObject = {
      "id": this.row['id'],
      "name": this.row['name'],
      "description": this.row['description']
    }
    this.service.modifyTasks(this.row["id"],savedObject).subscribe(()=>{
      delete this.cloneRow;
      this.editProject = false;

    }, () => {
      console.error('Obiect gol sau invalid');
      this.row = this.cloneRow;
      this.editProject = false;
      delete this.cloneRow;
    });


  }
  loadElements(){
    this.service.getSubtasks(this.token,this.idTask).subscribe(result =>{
      this.data = result;
      this.data.forEach((row: { deadline: string | number | Date; }) =>{
        row.deadline = new Date(row.deadline);
      });
      this.loading = false;
    },error => {
      console.log(error);
      this.loading = false;
    })
  }
  onRowEditInit(row: any,index:any) {
    this.clonedRows[index] = {...this.data[index]};
    this.saveRow = "edit";
    this.addRow = false;

  }

  onRowEditSave(row: any,index:any) {
    this.loading = true;
    if(this.saveRow == "edit"){
      const savedObject = {
        "id":row['id'],
        "projectId": row['projectId'],
        "parentId": row['parentId'],
        "title": row['title'],
        "description": row['description'],
        "deadline":row['deadline']
      }
      console.log(savedObject);
      this.service.modifyTasks(row["id"],savedObject).subscribe(()=>{

        this.loading = false;
        this.addRow = true;
      }, () => {
        console.error('Obiect gol sau invalid');
        this.addRow = true;
        this.loading = false;
      });


    }
    else{
      const savedObject = {
        "projectId": this.idProject,
        "parentId": this.idTask,
        "title": row['title'],
        "description": row['description'],
        "deadline":row['deadline']
      }
      console.log(savedObject);
      this.service.addTasks(savedObject).subscribe(result=>{
        this.loadElements();
        this.loading = false;
        this.addRow = true;
      }, error => {
        console.error('Obiect gol sau invalid');
        this.loading = false;
        this.data.shift();
        this.addRow = true;
      });
    }

    delete this.clonedRows[index];
  }

  onRowEditCancel(row: any, rowIndex: any) {
    if(this.saveRow == "edit"){
      this.data[rowIndex] = this.clonedRows[rowIndex];
      delete this.clonedRows[rowIndex];
    }
    else{
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

  back(){
    this.emitter.emit("back");
  }

  deleteTask(){
    this.taskService.removeTask(this.idTask).subscribe(result=>{
      this.back();
    },error => {
      console.error("Stergerea nu a putut fi realizata");
    })
  }

  deleteSubTask(row:any){
    this.taskService.removeTask(row.id).subscribe(result=>{
      this.loadElements();
    },error => {
      console.error("Stergerea nu a putut fi realizata");
    })
  }

}
