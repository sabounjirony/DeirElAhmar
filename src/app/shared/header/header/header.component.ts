import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/primeng';
import { Router } from '@angular/router';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.css']
})

export class HeaderComponent  {

    constructor(private router: Router) {}
    
    items: MenuItem[];

    ngOnInit() {
        this.items = [
            {
                label: 'Business',
                icon: 'fa-briefcase',
                items: [
                    {label: 'Test',icon: 'fa-user', routerLink: 'home/business/test'},
                ]
            },
            {
                label: 'Setup',
                icon: 'fa-gear',
                items: [
                    {label: 'Clients',icon: 'fa-user', routerLink: 'home/setup/client'},
                    {label: 'Teams',icon: 'fa-users'},
                    {label: 'Absence',icon: 'fa-phone'},
                    {label: 'Licenses',icon: 'fa-certificate'},
                    {label: 'Environments',icon: 'fa-phone'}
                ]
            },
            {
                label: 'System', icon: 'fa-cogs',
                items: [
                    {label: 'Descriptions', icon: 'fa-file-o', routerLink: 'home/system/descriptions'},
                    {label: 'Logging', icon: 'fa-eye',
                    items: [
                        {label: 'Errors', icon: 'fa-minus-circle', routerLink: 'home/system/errors'},
                        {label: 'Events', icon: 'fa-warning', routerLink: 'home/system/events'}
                    ]},
                    {label: 'Security', icon: 'fa-shield',
                    items: [
                        {label: 'Modules', icon: 'fa-puzzle-piece', routerLink: 'home/system/modules'},
                        {label: 'Roles', icon: 'fa-user-secret', routerLink: 'home/system/roles'},
                        {label: 'Users', icon: 'fa-users', routerLink: 'home/system/users'},
                        {label: 'Menu', icon: 'fa-bars', routerLink: 'home/system/menus'}
                    ]},
                    {label: 'List of values', icon: 'fa-list-ul'},
                    {label: 'Codes', icon: 'fa-list-alt', routerLink: 'home/system/codes'},
                ]
            }
        ];
    }
    onSignOut() {
        this.router.navigate(['logout']);
    }
}