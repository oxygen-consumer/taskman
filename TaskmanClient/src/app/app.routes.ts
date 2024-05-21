import { Routes } from '@angular/router';
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {StartPageComponent} from "./start/start-page.component";
import {ProjectsComponent} from "./projects/projects.component";
import {AuthGuard} from "./service/auth-service";

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'start', component: StartPageComponent },
  {path:'dashboard',component:ProjectsComponent ,canActivate: [AuthGuard]},
  { path: '**', redirectTo: '/start', pathMatch: 'full' }
];
