import {Component,OnInit} from '@angular/core';
import {LoginServiceService} from "../service/login-service.service";
import {ProjectService} from "../service/project-service.service";


@Component({
  selector: 'app-projects',
  standalone: true,
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent {
  data:any;
  token:any;
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
}
