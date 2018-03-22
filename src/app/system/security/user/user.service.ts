import { Injectable } from '@angular/core';
<<<<<<< HEAD
import { Headers, Http } from "@angular/http";
import { Observable }     from 'rxjs/Observable';
=======
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Headers, Http } from "@angular/http";
import { Observable } from 'rxjs/Observable';
>>>>>>> 

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

import { User } from './user';
<<<<<<< HEAD
=======
import { Globals } from './../../../app.globals';
>>>>>>> 

@Injectable()
export class UserService {

<<<<<<< HEAD
  private serviceUrl = 'api/module';

  constructor(private http: Http) { }
  
  search(term: string): Observable<User[]> {
    return this.http
               .get(`${this.serviceUrl}/?name=${term}`)
               .map(response => response.json().data as User[]);
  }

  getHeroes(): Promise<User[]> {
    return this.http.get(this.serviceUrl)
               .toPromise()
               .then(response => response.json().data as User[])
               .catch(this.handleError);
  }

  getHero(id: number): Promise<User> {
    const url = `${this.serviceUrl}/${id}`;
    return this.http.get(url)
      .toPromise()
      .then(response => response.json().data as User)
      .catch(this.handleError);
  }

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error); // for demo purposes only
    return Promise.reject(error.message || error);
  }

  private headers = new Headers({'Content-Type': 'application/json'});
  update(obj: User): Promise<User> {
    const url = `${this.serviceUrl}/${obj.id}`;
    return this.http
      .put(url, JSON.stringify(obj), {headers: this.headers})
      .toPromise()
      .then(() => obj)
      .catch(this.handleError);
  }

  create(name: string): Promise<User> {
    return this.http
      .post(this.serviceUrl, JSON.stringify({name: name}), {headers: this.headers})
      .toPromise()
      .then(res => res.json().data as User)
      .catch(this.handleError);
  }

  delete(id: number): Promise<void> {
    const url = `${this.serviceUrl}/${id}`;
    return this.http.delete(url, {headers: this.headers})
      .toPromise()
      .then(() => null)
      .catch(this.handleError);
  }

  Authenticate(credentials: any): Observable<any>{
    return this.http
    .get('http://localhost:3000/user/data.json/', credentials)
    .map(response => response.json().data as any);
  }
=======
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
      .post(url, JSON.stringify({ "username": userName}), this.globals.httpOptions);
  }

>>>>>>> 
}