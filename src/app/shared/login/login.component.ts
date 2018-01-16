import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgForm, FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from './../../system/security/user/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  frm: FormGroup;
  model: any;
  blockDocument: boolean = false;

  constructor(private router: Router, private userService: UserService) {
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

  onSubmit() {
    this.userService.Authenticate({'username': this.model.userName, 'password': this.model.Password}).subscribe(
      (response) => {
        alert('success');
      },
      (error) => {
        alert('fail');
      }
    );
    //this.blockDocument = true;
  }
}