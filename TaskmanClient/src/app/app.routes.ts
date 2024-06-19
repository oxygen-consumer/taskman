import { Routes } from '@angular/router';
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {StartPageComponent} from "./start/start-page.component";
import {AuthGuard} from "./service/auth-service";
import {ProjectsMasterComponent} from "./projects-master/projects-master.component";
import {ProjectsComponent} from "./projects-master/projects/projects.component";
import {DashboardComponent} from "./dashboard/dashboard.component";
import {LandingComponent} from "./landing/landing.component";

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'start', component: StartPageComponent },
  {path:'dashboard',component:DashboardComponent ,canActivate: [AuthGuard] , children:[
      { path: 'projects', component: ProjectsMasterComponent, canActivate:[AuthGuard] },
      { path: 'landing', component: LandingComponent, canActivate:[AuthGuard] },
    ]},
  { path: '**', redirectTo: '/start', pathMatch: 'full' }
];
