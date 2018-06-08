import { Injectable } from "@angular/core";
import { Effect, Actions } from '@ngrx/effects';
import { User } from './userModel';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/fromPromise';
import 'rxjs/add/observable/of';

import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/delay';

import * as userActions from './userActions';
export type Action = userActions.All;

@Injectable()
export class userEffects {
    constructor(private actions: Actions) { }

    // @Effect()
    // getUser: Observable<Action> = this.actions.ofType(userActions.GET_USER)

    //     .map((action: userActions.GetUser) => action.payload)
    //     .switchMap(payload => this.afAuth.authState)
    //     .delay(2000) //delay to show loading spinner
    //     .map(authData => {
    //         if (authData) {
    //             //User logged in
    //             const user = new User(authData.uid, authData.displayName);
    //             return new userActions.Authenticated(user);
    //         } else {
    //             //User not logged in
    //             return new userActions.NotAuthenticated();
    //         }
    //     })
    //     .catch(err => Observable.of(new userActions.AuthError({ error: err.message })));


    // @Effect()
    // login: Observable<Action> = this.actions.ofType(userActions.GOOGLE_LOGIN)
    //     .map((action: userActions.GoogleLogin) => action.payload)
    //     .switchMap(payload => {
    //         return Observable.fromPromise(this.googleLogin());
    //     })
    //     .map(credential => {
    //         // successful login
    //         return new userActions.GetUser();
    //     })
    //     .catch(err => { return Observable.of(new userActions.AuthError({ error: err.message })); });

    // private googleLogin(): Promise<any> {
    //     const provider = new firebase.auth.GoogleAuthProvider();
    //     return this.afAuth.auth.signInWithPopup(provider);
    // }

    // @Effect()
    // logout: Observable<Action> = this.actions.ofType(userActions.LOGOUT)
    //     .map((action: userActions.Logout) => action.payload)
    //     .switchMap(payload => {
    //         return Observable.of(this.afAuth.auth.signOut());
    //     })
    //     .map(authData => {
    //         return new userActions.NotAuthenticated();
    //     })
    //     .catch(err => Observable.of(new userActions.AuthError({ error: err.message })));

}