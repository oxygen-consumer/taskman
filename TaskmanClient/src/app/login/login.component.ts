import { Component } from '@angular/core';
import {RouterLink, RouterOutlet} from '@angular/router';
import {FormsModule} from "@angular/forms";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterOutlet, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  username: any;
  password: any;

  login() {
    console.log('Username:', this.username);
    console.log('Password:', this.password);
  }
}
