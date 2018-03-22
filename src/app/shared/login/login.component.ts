<<<<<<< HEAD
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { NgForm, FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

import { ICodeStore } from './../../system/code/store/code.Store';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
=======
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { UserService } from './../../system/security/user/user.service';

import { Globals } from './../../app.globals';

@Component({
  templateUrl: 'login.component.html',
>>>>>>> 
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
<<<<<<< HEAD

  frm: FormGroup;
  model: any;
  blockDocument: boolean = false;

  message$: Observable<string>;

  constructor(private router: Router, private store: Store<ICodeStore>) {

    this.message$ = this.store.select('message');
    this.frm = new FormGroup({
      "txtUserName": new FormControl('', [Validators.required, Validators.minLength(4)]),
      "txtPassword": new FormControl('', Validators.required)
    });

    this.model = {
      UserName: "",
      Password: ""
    };

  }

  ngOnInit() {
    alert('on init');
    console.log(this.frm);
  }

  spanishMessage() {
    this.store.dispatch({ type: 'SPANISH' });
  }

  frenchMessage() {
    this.store.dispatch({ type: 'FRENCH' });
  }
  // onSubmit() {
  //   this.userService.Authenticate({'username': this.model.userName, 'password': this.model.Password}).subscribe(
  //     (response) => {
  //       alert('success');
  //     },
  //     (error) => {
  //       alert('fail');
  //     }
  //   );
  //   //this.blockDocument = true;
  // }
}
=======
  loginForm: FormGroup;

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      'userName': new FormControl('', [Validators.required, Validators.minLength(3)]),
      'password': new FormControl('', [Validators.required, Validators.minLength(3)])
    });
  }

  constructor(private fb: FormBuilder, private userService: UserService, private globals: Globals, private router: Router) { }

  login() {
    const val = this.loginForm.value;
    var results;
    if (val.userName && val.password) {
      this.userService.authenticate(val.userName, val.password)
      .subscribe(
        response => {
          this.successfulLogin(val.userName, response);
        },
        error => {
          this.failedLogin(error);
        }
      );
    }
  }

  private successfulLogin(userName, token){
    localStorage.setItem('userName', userName);
    localStorage.setItem('token', JSON.stringify(token));
    this.router.navigateByUrl('/dashboard');
  }

  private failedLogin(err) {
    localStorage.removeItem("token");
    this.globals.handleSvcError(err);
  }
}
>>>>>>> 
