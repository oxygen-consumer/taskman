import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {ProjectService} from "../../../service/project-service.service";
import {Table, TableModule} from "primeng/table";
import {InputTextModule} from "primeng/inputtext";
import {FormsModule} from "@angular/forms";
import {RippleModule} from "primeng/ripple";
import {ButtonModule} from "primeng/button";
import {DatePipe, NgIf} from "@angular/common";
import {ProgressSpinnerModule} from "primeng/progressspinner";
import {ToastModule} from "primeng/toast";
import {LoginServiceService} from "../../../service/login-service.service";
import {TaskService} from "../../../service/task-service.service";
import {CalendarModule} from "primeng/calendar";
import {BrowserModule} from "@angular/platform-browser";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@Component({
  selector: 'app-tasks',
  standalone: true,
  templateUrl: './tasks.component.html',
  imports: [
    TableModule,
    InputTextModule,
    FormsModule,
    RippleModule,
    ButtonModule,
    NgIf,
    ProgressSpinnerModule,
    ToastModule,
    CalendarModule,
  ],
  styleUrl: './tasks.component.scss'
})
export class TasksComponent {
  @Input() row:any;
  cloneRow:any;
  @Output() emitter = new EventEmitter<any>();
  data:any;
  token:any;
  loading = true;
  @ViewChild('dt') table: Table;
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

  constructor(private service:TaskService , private loginService:LoginServiceService, private projectService:ProjectService){
    this.token = sessionStorage.getItem(this.accesToken);
   }
  onButtonClick() {
    this.emitter.emit();
  }

    ngOnInit(){
      this.loading = true;
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
    this.service.getTasks(this.token,this.row.id).subscribe(result =>{
      this.data = result;
      this.data.forEach((row: { deadline: string | number | Date; }) =>{
        row.deadline = new Date(row.deadline);
      });
      this.loading = false;
    },error => {
      console.log(error);
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
         "projectId": this.row['id'],
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
        "projectId": this.row['id'],
        "title": row['title'],
        "description": row['description'],
        "deadline":row['deadline']
      }

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


  onAddUser(username: any){
    this.projectService.addUser(this.row.id,username).subscribe(()=>{
    }, () => {
      console.error('Input gol sau invalid');
    });
  }

  onRemoveUser(username: any){
    this.projectService.removeUser(this.row.id,username).subscribe(()=>{
    }, () => {
      console.error('Input gol sau invalid');
    });
  }

  onPromoteUser(username: any){
    this.projectService.promoteUser(this.row.id,username).subscribe(()=>{
    }, () => {
      console.error('Input gol sau invalid');
    });
  }

  onDemoteUser(username: any){
    this.projectService.demoteUser(this.row.id,username).subscribe(()=>{
    }, () => {
      console.error('Input gol sau invalid');
    });
  }

  onTransferOwnership(username: any){
    this.projectService.transferOwnership(this.row.id,username).subscribe(()=>{
    }, () => {
      console.error('Input gol sau invalid');
    });
  }


}



export interface Projects {
  name?: string;
  description?: string;

}
