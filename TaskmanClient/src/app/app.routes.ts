import { Routes } from '@angular/router';
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {StartPageComponent} from "./start/start-page.component";

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'start', component: StartPageComponent },
  { path: '**', redirectTo: '/start', pathMatch: 'full' }
];
