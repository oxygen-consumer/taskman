import {Component, ElementRef, ViewChild, ViewChildren} from '@angular/core';
import {RouterOutlet } from '@angular/router';
import {FormBuilder, FormGroup,ReactiveFormsModule, Validators} from "@angular/forms";
import {LoginServiceService} from "../service/login-service.service";
import {HttpClientModule} from "@angular/common/http";
import {NgIf} from "@angular/common";

@Component({
  selector: 'register-component',
  standalone: true,
  imports: [RouterOutlet, ReactiveFormsModule, HttpClientModule, NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  title: any;
  password: string | undefined;
  email: string | undefined;
  isMessageShown:boolean = false;
  message:string | undefined;
  @ViewChildren('para') firstParagraph!: any;

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
        form.reset();
        this.message = "Valoare introdusa corect";
        //this.firstParagraph.nativeElement.style.backgroundColor = 'green';
        this.isMessageShown = true;
      }, error => {
        console.error('Error occured');
        this.message = "Eroare la introducerea datelor";
        //this.firstParagraph.nativeElement.style.backgroundColor = 'red';
        this.isMessageShown = true;
      });
    } else {
      console.log("Form is not valid.");
      this.message = "Form invalid";
      //this.firstParagraph.nativeElement.style.backgroundColor = 'red';
      this.isMessageShown = true;
    }
  }

}
