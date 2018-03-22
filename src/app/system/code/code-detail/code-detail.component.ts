import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgForm, FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';

import { InputMaskModule } from 'primeng/primeng';

import { Code } from './../code';

@Component({
  selector: 'app-code-detail',
  templateUrl: './code-detail.component.html',
  styleUrls: ['./code-detail.component.css']
})

export class CodeDetailComponent implements OnInit {

  frm: FormGroup;
  model: Code;
  id: string;
  mode: string;

  // private paramsSubscription: Subscription;
  // private queryparamsSubscription: Subscription;

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    this.frm = new FormGroup({
      "ddlCategory": new FormControl('', Validators.required),
      "txtName": new FormControl('', Validators.required),
      "txtValue": new FormControl('', Validators.required),
      "txtDescription": new FormControl(),
      "ddlStatus": new FormControl('', Validators.required),
      "chkIsProtected": new FormControl(),
    });

    this.model = {
      Category: "1",
      Name: "",
      Value: "",
      Description: "",
      IsProtected: true,
      Status: "A"
    };

    // //Subscribing to the params observable
    // this.paramsSubscription = this.activatedRoute.params.subscribe(
    //   (params: any) => {
    //     this.id = params["id"];
    //     this.mode = params["mode"];
    //   }
    // );

    //Subscribing to the query params observable
    // this.queryparamsSubscription = this.activatedRoute.queryParams.subscribe(
    //   (params: any) => {
    //     this.id = params["id"];
    //     this.mode = params["mode"];
    //   }
    // );

  }

  ngOnInit() { }

  //If we do not implement this a memory leak will occur since the subscription will always stay there
  // ngOnDestroy() { 
  //   this.paramsSubscription.unsubscribe();
  //   //this.queryparamsSubscription.unsubscribe();
  //  }

  onSubmit() {
    console.log(this.frm);
  }
}