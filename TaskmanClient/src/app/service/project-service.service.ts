import { Injectable } from '@angular/core';
import {catchError, Observable, switchMap} from "rxjs";
import {HttpClient, HttpClientModule, HttpHeaders} from "@angular/common/http";
import {LoginServiceService} from "./login-service.service";

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  constructor(private http:HttpClient,private service:LoginServiceService) { }
  baseLink :string = "http://127.0.0.1:5096"

  getProjects(token: any): Observable<any> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
      return this.http.get<any>(this.baseLink + "/api/Projects", {headers});
  }

  // @ts-ignore
  modifyProject(id: any, data: any): Observable<any> {
    return this.service.getToken().pipe(
      switchMap(token => {
        const headers = new HttpHeaders({
          'Authorization': `Bearer ${token}`
        });
        return this.http.put<any>(this.baseLink + "/api/Projects/" + id, data, { headers }).pipe(
          catchError(error => {
            console.error('Error modifying project:', error);
            // You can handle the error here or rethrow it if necessary
            throw error;
          })
        );
      })
    );
  }
  // @ts-ignore

  addProject(data: any): Observable<any> {
    return this.service.getToken().pipe(
      switchMap(token => {
        const headers = new HttpHeaders({
          'Authorization': `Bearer ${token}`
        });
        return this.http.post<any>(this.baseLink + "/api/Projects/", data, { headers }).pipe(
          catchError(error => {
            console.error('Error adding project:', error);
            // You can handle the error here or rethrow it if necessary
            throw error;
          })
        );
      })
    );
  }



}
