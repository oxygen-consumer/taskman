import {Component, OnInit, QueryList, ViewChild, ViewChildren} from '@angular/core';
import {LoginServiceService} from "../service/login-service.service";
import {ProjectService} from "../service/project-service.service";
import {Table, TableModule} from "primeng/table";
import {InputTextModule} from "primeng/inputtext";
import {FormBuilder, FormsModule} from "@angular/forms";
import {RippleModule} from "primeng/ripple";
import {ButtonModule} from "primeng/button";
import {NgIf} from "@angular/common";
import {ProgressSpinnerModule} from "primeng/progressspinner";
import {ToastModule} from "primeng/toast";
import {NavigationComponent} from "../navigation/navigation.component";
import {Router, RouterLink, RouterOutlet} from "@angular/router";


@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  imports: [
    TableModule,
    InputTextModule,
    FormsModule,
    RippleModule,
    ButtonModule,
    NgIf,
    RouterOutlet,
    RouterLink,
    ProgressSpinnerModule,
    ToastModule,
    NavigationComponent,
  ],
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {

  constructor(private router:Router){

  }
  ngOnInit(){

  }
}
