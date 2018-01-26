import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgForm, FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';

import { DataTableModule, SharedModule } from 'primeng/primeng';

import { AppFunctions } from './../../../app.functions';
import { Code } from './../code';

@Component({
  selector: 'app-code-list',
  templateUrl: './code-list.component.html',
  styleUrls: ['./code-list.component.css']
})

export class CodeListComponent implements OnInit {
  frm: FormGroup;
  model: Code;
  cols: any[];
  codes: Code[];
  constructor(private appFunctions: AppFunctions) {
    this.frm = new FormGroup({
      "ddlCategory": new FormControl('', Validators.required),
      "txtName": new FormControl('', Validators.required),
      "txtValue": new FormControl('', Validators.required),
      "txtDescription": new FormControl(),
      "ddlStatus": new FormControl('', Validators.required),
      "chkIsProtected": new FormControl(),
    });

    this.model = {
      Category: "",
      Name: "",
      Value: "",
      Description: "",
      IsProtected: false,
      Status: ""
    };
  }
  // id: string;
  // constructor(private router: Router, private activatedRoute: ActivatedRoute) {
  //   this.id = this.activatedRoute.snapshot.params["id"];
  // }

  ngOnInit() {
    this.cols = [
      { field: 'category', header: 'Category', sortable: true },
      { field: 'code', header: 'Code' },
      { field: 'value1', header: 'Value1' },
      { field: 'value2', header: 'Value2' },
      { field: 'status', header: 'Status' },
      { field: 'protected', header: 'Protected' }];
    this.OnSearch();
  }

  OnSearch() {
    //this.codeService.loadPaging(
    //this.codeService.loadPaging(this.frm.value).then(codes => this.codes = codes);
  }
}