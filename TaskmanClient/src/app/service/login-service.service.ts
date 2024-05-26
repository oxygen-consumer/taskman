import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient, HttpClientModule} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class LoginServiceService {

  accesToken = "acces_token";
  refreshToken = "refresh_token";
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

  getToken(): string | null {
    return sessionStorage.getItem(this.accesToken);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }


}
