import { Injectable} from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class RouteGameGuardService implements CanActivate {

  constructor(private authService: AuthService,
    private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (state && state.url === "game")
      return true;

    var status = this.authService.userDataBuffer;
    if (status && status.isInGame)
      return this.router.parseUrl("/game");

    return true;
  }
}
