import { Injectable} from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { IRoute, RootRoutes } from './route.const';

@Injectable()
export class NavMenuService{
  public RouteChanged: BehaviorSubject<IRoute[]> = new BehaviorSubject([]);

  constructor(private authService: AuthService) {
    authService.authChanged.subscribe(
      () => this.RouteChanged.next(this.Routes));
  }

  public ClickNav(navName: string) {
    if (navName === "登出") {
      this.authService.Logout();
      alert("已登出");
    }
  }

  public get Routes(): IRoute[]  {
    var routes = [];
    var authService = this.authService;
    RootRoutes.forEach((v) => {
      if (v.redirectTo === undefined) {
        routes.push(v);
        routes[routes.length - 1].onNav = authService.isNavShow(v.name);
      }
    });
    return routes;
  }

}
