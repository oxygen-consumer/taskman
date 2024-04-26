import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient, HttpClientModule} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class LoginServiceService {
  constructor(private http:HttpClient) { }

  onLogin(data: any): Observable<any> {
    return this.http.post<any>("http://127.0.0.1:5096/account/login", data);
  }

  onRegister(data: any): Observable<any> {
    return this.http.post<any>("http://127.0.0.1:5096/account/register", data);
  }


}
