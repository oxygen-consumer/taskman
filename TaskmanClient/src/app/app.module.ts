import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {AppComponent} from "./app.component";
import {BrowserModule} from "@angular/platform-browser";
import {Router, RouterModule} from "@angular/router";
import {FormsModule} from "@angular/forms";
import {LoginComponent} from "./login/login.component";
import {HeaderComponent} from "./header/header.component";



@NgModule({
  declarations: [

  ],
  exports: [
    HeaderComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    RouterModule,
    FormsModule,
    LoginComponent,
    HeaderComponent,
    AppComponent,

  ]
})
export class AppModule { }
