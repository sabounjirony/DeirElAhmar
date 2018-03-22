import { Injectable } from '@angular/core';
import { Headers, Http } from "@angular/http";
import { Observable }     from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

import { Error } from './error';

@Injectable()
export class ErrorService {

  private serviceUrl = 'api/code';

  constructor(private http: Http) { }
  
  search(term: string): Observable<Error[]> {
    return this.http
               .get(`${this.serviceUrl}/?name=${term}`)
               .map(response => response.json().data as Error[]);
  }

  getHeroes(): Promise<Error[]> {
    return this.http.get(this.serviceUrl)
               .toPromise()
               .then(response => response.json().data as Error[])
               .catch(this.handleError);
  }

  getHero(id: number): Promise<Error> {
    const url = `${this.serviceUrl}/${id}`;
    return this.http.get(url)
      .toPromise()
      .then(response => response.json().data as Error)
      .catch(this.handleError);
  }

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error); // for demo purposes only
    return Promise.reject(error.message || error);
  }

  private headers = new Headers({'Content-Type': 'application/json'});
  update(hero: Error): Promise<Error> {
    const url = `${this.serviceUrl}/${hero.id}`;
    return this.http
      .put(url, JSON.stringify(hero), {headers: this.headers})
      .toPromise()
      .then(() => hero)
      .catch(this.handleError);
  }

  create(name: string): Promise<Error> {
    return this.http
      .post(this.serviceUrl, JSON.stringify({name: name}), {headers: this.headers})
      .toPromise()
      .then(res => res.json().data as Error)
      .catch(this.handleError);
  }

  delete(id: number): Promise<void> {
    const url = `${this.serviceUrl}/${id}`;
    return this.http.delete(url, {headers: this.headers})
      .toPromise()
      .then(() => null)
      .catch(this.handleError);
  }
}