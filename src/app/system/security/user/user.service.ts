import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Headers, Http } from "@angular/http";
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

import { User } from './user';
import { Globals } from './../../../app.globals';

@Injectable()
export class UserService {

  private controller = 'user';

  constructor(private http: HttpClient, private globals: Globals) { }

  get(id: number): Observable<HttpResponse<User>> {
    var url = `${this.globals.serviceRootUrl}/${this.controller}/get/${id}`;
    return this.http
      .get<User>(url, { observe: 'response' });
  }

  search(filter: string): Observable<User> {
    var url = `${this.globals.serviceRootUrl}/${this.controller}/search`;
    return this.http
      .post<User>(url, JSON.stringify({ "filter": filter }), this.globals.httpOptions);
  }

  post(obj: User) {
    var url = `${this.globals.serviceRootUrl}/${this.controller}/post`;
    return this.http
      .put<User>(url, JSON.stringify(obj), this.globals.httpOptions);
  }

  put(obj: User): Observable<User> {
    var url = `${this.globals.serviceRootUrl}/${this.controller}/put`;
    return this.http
      .put<User>(url, JSON.stringify(obj), this.globals.httpOptions);
  }

  delete(id: number): Observable<{}> {
    var url = `${this.globals.serviceRootUrl}/${this.controller}/delete/${id}`;
    return this.http
      .delete(url, this.globals.httpOptions);
  }

  authenticate(userName: string, password: string): Observable<object> {
    var url = `${this.globals.serviceRootUrl}/${this.controller}/authenticate`
    return this.http
      .post(url, JSON.stringify({ "username": userName, "password": password }), this.globals.httpOptions);
  }

  logout(userName: string): Observable<object> {
    var url = `${this.globals.serviceRootUrl}/${this.controller}/logout`
    return this.http
      .post(url, JSON.stringify({ "username": userName }), this.globals.httpOptions);
  }
}