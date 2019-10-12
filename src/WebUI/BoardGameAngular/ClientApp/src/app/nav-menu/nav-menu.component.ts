import { Component } from '@angular/core';
import { IRoute, RootRoutes } from './route.const';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  public Routes: IRoute[];
  isExpanded = false;

  constructor() {
    this.Routes = [];
    RootRoutes.forEach((v) => {
      if (!v.redirectTo && (
        !v.canActivate
      ))
        this.Routes.push(v);
    });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
