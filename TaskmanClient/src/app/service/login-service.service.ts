import { Injectable } from '@angular/core';
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class LoginServiceService {
  private http: any;

  constructor() { }

  onLogin(data: any): Observable<any> {
    // @ts-ignore
    return this.http.post<any>("localhost:5096/account/login", data);
  }

  onRegister(data: any): Observable<any> {
    // @ts-ignore
    return this.http.post<any>("localhost:5096/account/register", data);
  }


}
