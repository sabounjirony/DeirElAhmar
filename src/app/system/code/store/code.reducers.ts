import { Action } from '@ngrx/store';
import { Code } from '../../code/code';
import * as CodeActions from './code.Actions';

const initialState: Code =
    {
        Category: "",
        Name: "",
        Value: "",
        Description: "",
        Status: "",
        IsProtected: false
    };

export function codeReducer(state: Code = initialState, action: CodeActions.CodeActionTypes) {

    switch (action.type) {
        case CodeActions.ADD_CODE:
            { return state; }
        default:
            { return state; }
    }
}