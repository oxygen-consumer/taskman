import { Injectable } from '@angular/core';
import {CanActivate, Router, UrlTree} from '@angular/router';
import {LoginServiceService} from "./login-service.service";
import {Observable,pipe} from "rxjs";
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: LoginServiceService, private router: Router) {}

  canActivate(): Observable<boolean | UrlTree> {
    return this.authService.isAuthenticated().pipe(
      map(result => {
        if (result == null) {
          return this.router.createUrlTree(['/login']);
        } else {
          return true;
        }
      })
    );
  }
}
