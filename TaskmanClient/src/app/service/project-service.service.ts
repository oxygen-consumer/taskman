import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {HttpClient, HttpClientModule, HttpHeaders} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  constructor(private http:HttpClient) { }
  baseLink :string = "http://127.0.0.1:5096"
  getProjects(token: any): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
      return this.http.get<any>(this.baseLink + "/api/Projects", {headers});
  }



}
