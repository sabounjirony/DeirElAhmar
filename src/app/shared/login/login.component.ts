import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgForm, FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AppService } from '../../services/app.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {

  frm: FormGroup;
  model: any;
  blockDocument: boolean = false;

  constructor(private router: Router, private appService: AppService) {
    this.frm = new FormGroup({
      "txtUserName": new FormControl('', [Validators.required, Validators.minLength(10)]),
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
    this.appService.Authenticate({'username': this.model.userName, 'password': this.model.Password}).subscribe(
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