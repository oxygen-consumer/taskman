import {Component} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {NgOptimizedImage} from "@angular/common";

@Component({
  selector: 'app-navigation',
  standalone: true,
  imports: [RouterOutlet, NgOptimizedImage],
  templateUrl: './navigation.component.html',
  styleUrl: './navigation.component.scss'
})
export class NavigationComponent {
  title = 'TaskmanClient';

  constructor(private router: Router) {

  }

  logout(): void {
    sessionStorage.removeItem('user_email');
    sessionStorage.removeItem('acces_token');
    sessionStorage.removeItem('refresh_token');

    this.router.navigate(['/start']);
  }

}
