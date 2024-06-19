import { Injectable } from '@angular/core';
import {catchError, Observable, switchMap} from "rxjs";
import {HttpClient, HttpClientModule, HttpHeaders} from "@angular/common/http";
import {LoginServiceService} from "./login-service.service";

@Injectable({
  providedIn: 'root'
})
export class SubtaskService {
  constructor(private http:HttpClient,private service:LoginServiceService) { }
  baseLink :string = "http://127.0.0.1:5096"

  getSubtasks(token: any,id:any): Observable<any> {
    return this.service.getToken().pipe(
      switchMap(token => {
        const headers = new HttpHeaders({
          'Authorization': `Bearer ${token}`
        });
        return this.http.get<any>(this.baseLink + "/api/Tasks/"+id+"/get_subtasks/", {headers}).pipe(
          catchError(error => {
            console.error('Error modifying project:', error);
            throw error;
          })
        );
      })
    );

  }

  // @ts-ignore
  modifyTasks(id: any, data: any): Observable<any> {
    return this.service.getToken().pipe(
      switchMap(token => {
        const headers = new HttpHeaders({
          'Authorization': `Bearer ${token}`
        });
        return this.http.put<any>(this.baseLink + "/api/Tasks/", data, { headers }).pipe(
          catchError(error => {
            console.error('Error modifying project:', error);
            throw error;
          })
        );
      })
    );
  }
  // @ts-ignore

  addTasks(data: any): Observable<any> {
    return this.service.getToken().pipe(
      switchMap(token => {
        const headers = new HttpHeaders({
          'Authorization': `Bearer ${token}`
        });
        return this.http.post<any>(this.baseLink + "/api/Tasks/", data, { headers }).pipe(
          catchError(error => {
            console.error('Error adding project:', error);
            throw error;
          })
        );
      })
    );
  }



}
