import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { NgForm, FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';

import { ICodeStore } from './../../system/code/codeStore';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

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