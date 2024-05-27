import {Injectable} from '@angular/core';
import {map, Observable, of} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class LoginServiceService {

  accesToken = "acces_token";
  refreshToken = "refresh_token";
  userEmail = "user_email";
  constructor(private http:HttpClient) { }

  onLogin(data: any): Observable<any> {
    return this.http.post<any>("http://127.0.0.1:5096/account/login", data);
  }

  onRegister(data: any): Observable<any> {
    return this.http.post<any>("http://127.0.0.1:5096/account/register", data);
  }

  logout(): void {
    sessionStorage.removeItem(this.accesToken);
  }

  getEmail():string|null{
    return sessionStorage.getItem(this.userEmail);
  }

  getToken(): Observable<string | null> {
    const refreshToken = sessionStorage.getItem(this.refreshToken);
    if (refreshToken === null) {
      return of(null); // Return null as an Observable
    }
    return this.http.post<any>("http://127.0.0.1:5096/account/refresh", { refreshToken }).pipe(
      map(response => {
        const accessToken = response.accessToken;
        const newRefreshToken = response.refreshToken;
        sessionStorage.setItem(this.accesToken, accessToken);
        sessionStorage.setItem(this.refreshToken, newRefreshToken);
        return accessToken;
      })
    );
  }
  isAuthenticated():Observable<string | null> {
    return this.getToken();
  }



}
