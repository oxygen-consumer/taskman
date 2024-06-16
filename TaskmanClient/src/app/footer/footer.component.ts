import {Component} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {NgOptimizedImage} from "@angular/common";

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [RouterOutlet, NgOptimizedImage],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  title = 'TaskmanClient';

  constructor(private router: Router) {

  }

}
