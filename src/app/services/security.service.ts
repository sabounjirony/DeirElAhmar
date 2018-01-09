import { Injectable, ViewContainerRef } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

@Injectable()
export class SecurityService {

    private headers = new Headers({ 'Content-Type': 'application/json' });
    private serviceUrl = 'api/code';

    constructor(private http: Http) { }

    Authenticate(credentials: any): Observable<any>{
        return this.http
        .get('https://testfirebase-f9324.firebaseio.com/data.json/', credentials)
        .map(response => response.json().data as any);
      }
}