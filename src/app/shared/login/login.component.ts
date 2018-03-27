import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

import { UserService } from './../../system/security/user/user.service';
import { Globals } from './../../app.globals';
import { ICodeStore } from '../../system/code/store/code.Store';

@Component({
  templateUrl: 'login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private userService: UserService, private globals: Globals, private router: Router) { }

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      'userName': new FormControl('', [Validators.required, Validators.minLength(3)]),
      'password': new FormControl('', [Validators.required, Validators.minLength(3)])
    });
  }

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

  private successfulLogin(userName, token) {
    localStorage.setItem('userName', userName);
    localStorage.setItem('token', JSON.stringify(token));
    this.router.navigateByUrl('/dashboard');
  }

  private failedLogin(err) {
    localStorage.removeItem("token");
    this.globals.handleSvcError(err);
  }
}