import { Injectable } from "@angular/core";
import { HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { ErrorObservable } from "rxjs/observable/ErrorObservable";

@Injectable()
export class Globals {

    //Application constants
    serviceRootUrl: string = 'http://localhost/api';   //Service root URL
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json',
        })
    };

    //Reset form layout
    OnReset(frm) {
        Object.keys(frm.controls).forEach(key => {
            frm.reset();
        });
    }

    handleSvcError(error: HttpErrorResponse) {
        if (error.error instanceof ErrorEvent) {
            // A client-side or network error occurred. Handle it accordingly.
            console.error('An error occurred:', error.error.message);
        } else {
            // The backend returned an unsuccessful response code.
            // The response body may contain clues as to what went wrong,
            console.error(
                `Backend returned code ${error.status}, ` +
                `body was: ${error.error}`);
        }
        // return an ErrorObservable with a user-facing error message
        return new ErrorObservable(
            'Something bad happened; please try again later.');
    };

}