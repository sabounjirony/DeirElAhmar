import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import { Observable } from 'rxjs/Observable';
import { Code } from '../../models/system/code';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

@Injectable()
export class CodeService {

  private headers = new Headers({ 'Content-Type': 'application/json' });
  private serviceUrl = 'api/code';

  constructor(private http: Http) { }

  loadSingle(id: number): Observable<Code>{
    return this.http
    .get(`${this.serviceUrl}/?id=${id}`)
    .map(response => response.json().data as Code);
  }

  loadPaging(search: any): Observable<Code[]> {
    search = JSON.stringify(search);
    return this.http
      .get(`${this.serviceUrl}/?search=${search}`)
      .map(response => response.json().data as Code[]);
  }

  loadSearch(search: any): Observable<Code[]> {
    search = JSON.stringify(search);
    return this.http
      .get(`${this.serviceUrl}/?search=${search}`)
      .map(response => response.json().data as Code[]);
  }

  Create123(toCreate: Code[]): Promise<Code> {

    return this.http
      .post('https://testfirebase-f9324.firebaseio.com/', toCreate, { headers: this.headers })
      .toPromise()
      .then(res => res.json().data as Code)
      .catch(this.handleError);
      
  }

  Create(toCreate: Code[]): Promise<Code> {
    return this.http
      .post(this.serviceUrl, toCreate, { headers: this.headers })
      .toPromise()
      .then(res => res.json().data as Code)
      .catch(this.handleError);
  }

  Update(toUpdate: Code[]): Promise<Code> {
    return this.http
      .put(this.serviceUrl, JSON.stringify(toUpdate), { headers: this.headers })
      .toPromise()
      .then(() => toUpdate)
      .catch(this.handleError);
  }

  Delete(toDelete: Code[]): Promise<void> {
    return this.http
      .delete(this.serviceUrl, { headers: this.headers })
      .toPromise()
      .then(() => null)
      .catch(this.handleError);
  }

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error); // for demo purposes only
    return Promise.reject(error.message || error);
  }

}