import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {FormBuilder, FormGroup, NgForm, ReactiveFormsModule, Validators} from "@angular/forms";
import {LoginServiceService} from "../service/login-service.service";
import {HttpClient, HttpClientModule} from "@angular/common/http";

@Component({
  selector: 'register-component',
  standalone: true,
  imports: [RouterOutlet, ReactiveFormsModule,HttpClientModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
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

  onSubmit(form: any) {
    const email = form.value.email;
    const password = form.value.password;
    const values = {
      'email': email,
      'password': password,
    };

    if (this.registerForm.valid) {
      this.loginService.onRegister(values).subscribe(result => {
        const data = result.body;

      }, error => {
        console.error('Error occured', error);
      });
    } else {
      console.log("Form is not valid.");
    }
  }
}
