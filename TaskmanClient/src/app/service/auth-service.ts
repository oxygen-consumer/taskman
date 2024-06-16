import {Injectable} from '@angular/core';
import {CanActivate, Router} from '@angular/router';
import {LoginServiceService} from "./login-service.service";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: LoginServiceService, private router: Router) {
  }

  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }
}
