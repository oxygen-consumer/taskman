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

}
