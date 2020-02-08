import { Injectable} from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class RouteRoomGuardService implements CanActivate {

  constructor(private authService: AuthService,
    private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (state && state.url === "gameroom")
      return true;

    var status = this.authService.userDataBuffer;
    if (status && status.isInRoom)
      return this.router.parseUrl("/gameroom");

    return true;
  }
}
