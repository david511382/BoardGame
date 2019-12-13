import { Component } from '@angular/core';
import { IRoute, RootRoutes } from './route.const';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  public Routes: IRoute[];
  isExpanded = false;

  constructor(private authService: AuthService) {
    authService.authChanged.subscribe(value => {
      this.reload();
    });

    this.reload();
  }

  reload() {
    this.Routes = [];
    var routes = this.Routes;
    var authService = this.authService;
    RootRoutes.forEach((v) => {
      if (v.redirectTo === undefined) {
        routes.push(v);
        routes[routes.length - 1].onNav = authService.isNavShow(v.name);
      }
    });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
