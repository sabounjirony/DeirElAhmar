import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})
export class ErrorComponent implements OnInit, OnDestroy {

  errorMessage: string;
  private paramsSubscription: Subscription;

  constructor(private router: Router, private activatedRoute: ActivatedRoute) {
    this.paramsSubscription = this.activatedRoute.params.subscribe(
      (params: any) => {
        this.errorMessage = params["message"];
      }
    );
  }

  ngOnInit() {
    if(this.errorMessage == undefined)
    {this.errorMessage = "GenericError";}
  }

  ngOnDestroy() {
    this.paramsSubscription.unsubscribe();
  }

  onBackClick() {
    history.back();
  }

}