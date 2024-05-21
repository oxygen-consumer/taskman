import {Component,OnInit} from '@angular/core';
import {LoginServiceService} from "../service/login-service.service";


@Component({
  selector: 'app-projects',
  standalone: true,
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent {
   constructor(private service:LoginServiceService){

   }
    ngOnInit(){
      console.log("buna");
    }
}
