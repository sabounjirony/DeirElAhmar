import { Injectable, ViewContainerRef } from '@angular/core';

@Injectable()
export class AppFunctions {

    OnReset(frm)
    {
        Object.keys(frm.controls).forEach(key => {
            frm.reset();
          });
    }
}