import { Injectable, ViewContainerRef } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import { Observable } from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

@Injectable()
export class AppService {

    private headers = new Headers({ 'Content-Type': 'application/json' });
    private serviceUrl = 'api/code';

    constructor(private http: Http) { }


}