import {Component} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {NgOptimizedImage} from "@angular/common";
import {FooterComponent} from "../footer/footer.component";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterOutlet, NgOptimizedImage, FooterComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  title = 'TaskmanClient';

  constructor(private router: Router) {

  }

  onButtonClick() {
    this.router.navigate(["/login"]);
  }
}
