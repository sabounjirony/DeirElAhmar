import {Action} from '@ngrx/store';
import { Code } from '../../code/code';

export const ADD_CODE = 'ADD_CODE';

export class AddCode implements Action {
    readonly type = ADD_CODE;
    payload: Code;
}

export type CodeActionTypes = AddCode;