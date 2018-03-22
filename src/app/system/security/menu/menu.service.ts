import { Injectable } from '@angular/core';
import { Headers, Http } from "@angular/http";
import { Observable }     from 'rxjs/Observable';

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

import { Menu } from './menu';

@Injectable()
export class MenuService {

  private serviceUrl = 'api/code';

  constructor(private http: Http) { }
  
  search(term: string): Observable<Menu[]> {
    return this.http
               .get(`${this.serviceUrl}/?name=${term}`)
               .map(response => response.json().data as Menu[]);
  }

  getHeroes(): Promise<Menu[]> {
    return this.http.get(this.serviceUrl)
               .toPromise()
               .then(response => response.json().data as Menu[])
               .catch(this.handleError);
  }

  getHero(id: number): Promise<Menu> {
    const url = `${this.serviceUrl}/${id}`;
    return this.http.get(url)
      .toPromise()
      .then(response => response.json().data as Menu)
      .catch(this.handleError);
  }

  private handleError(error: any): Promise<any> {
    console.error('An error occurred', error); // for demo purposes only
    return Promise.reject(error.message || error);
  }

  private headers = new Headers({'Content-Type': 'application/json'});
  update(obj: Menu): Promise<Menu> {
    const url = `${this.serviceUrl}/${obj.id}`;
    return this.http
      .put(url, JSON.stringify(obj), {headers: this.headers})
      .toPromise()
      .then(() => obj)
      .catch(this.handleError);
  }

  create(name: string): Promise<Menu> {
    return this.http
      .post(this.serviceUrl, JSON.stringify({name: name}), {headers: this.headers})
      .toPromise()
      .then(res => res.json().data as Menu)
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