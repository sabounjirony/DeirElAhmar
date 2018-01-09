import { Injectable, ViewContainerRef } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import { Observable } from 'rxjs/Observable';

import { User } from '../../../models/system/security/user';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

@Injectable()
export class UserService {

    private headers = new Headers({ 'Content-Type': 'application/json' });
    private serviceUrl = 'api/code';

    constructor(private http: Http) { }

      post(user: User): Observable<User>{
        return this.http
        .get('https://testfirebase-f9324.firebaseio.com/data.json/'+ user.id)
        .map(response => response.json().data as User);
      }

      put(user: User): Observable<User>{
        return this.http
        .get('https://testfirebase-f9324.firebaseio.com/data.json/'+ user.id)
        .map(response => response.json().data as User);
      }

      delete(user: User): Observable<User>{
        return this.http
        .get('https://testfirebase-f9324.firebaseio.com/data.json/'+ user.id)
        .map(response => response.json().data as User);
      }

      getAll(): Observable<User>{
        return this.http
        .get('https://testfirebase-f9324.firebaseio.com/data.json/')
        .map(response => response.json().data as User);
      }

      get(user: User): Observable<User>{
        return this.http
        .get('https://testfirebase-f9324.firebaseio.com/data.json/'+ user.id)
        .map(response => response.json().data as User);
      }
}