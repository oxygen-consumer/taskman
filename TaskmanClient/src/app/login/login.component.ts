import {Component, ElementRef, ViewChild, ViewChildren} from '@angular/core';
import {Router, RouterLink, RouterOutlet} from '@angular/router';
import {FormBuilder, FormGroup, FormsModule, NgForm, ReactiveFormsModule, Validators} from "@angular/forms";
import {LoginServiceService} from "../service/login-service.service";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterOutlet, FormsModule, RouterLink, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  registerForm: FormGroup;
  title: any;
  password: string | undefined;
  email: string | undefined;
  accesToken = "acces_token";
  refreshToken = "refresh_token";
  userEmail = "user_email";
  @ViewChild('loginForm') myForm!: NgForm;


  constructor(private fb: FormBuilder, private loginService: LoginServiceService , private router:Router) {
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  login(form:any) {
    console.log(form);
    const email = form.value.email;
    const password = form.value.password;
    const values = {
      'email': email,
      'password': password,
    };

    if(this.registerForm.valid){
      this.loginService.onLogin(values).subscribe(result => {
        const data = result;
        sessionStorage.setItem(this.accesToken,data.accessToken);
        sessionStorage.setItem(this.refreshToken,data.refreshToken);
        sessionStorage.setItem(this.userEmail,values.email);
        form.reset();
        this.router.navigate(["/dashboard"]);
      }, error => {
        console.error('Error occured');

      });
    }

  }
}
