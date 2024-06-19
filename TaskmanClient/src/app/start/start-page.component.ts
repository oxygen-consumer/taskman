import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {LoginServiceService} from "../service/login-service.service";
import {HttpClientModule} from "@angular/common/http";
import {HeaderComponent} from "../header/header.component";

@Component({
  selector: 'startpage-component',
  standalone: true,
  imports: [RouterOutlet, ReactiveFormsModule, HttpClientModule, HeaderComponent],
  templateUrl: './start-page.component.html',
  styleUrl: './start-page.component.scss'
})
export class StartPageComponent {
  registerForm: FormGroup;
  title: any;
  password: string | undefined;
  email: string | undefined;

  constructor(private fb: FormBuilder, private loginService: LoginServiceService) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

}
