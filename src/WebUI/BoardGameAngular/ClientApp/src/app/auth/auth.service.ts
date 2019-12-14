import { Injectable} from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',//singleton 
})
export class AuthService implements CanActivate {
  public authChanged: BehaviorSubject<boolean> = new BehaviorSubject(false);
  private readonly TOKEN_COOKIE_NAME: string = "token";

  constructor(private cookieService: CookieService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!this.token || this.token === "") {
      return false;
    }
    return true;
  }

  isNavShow(navName : string) :boolean {
    switch (navName) {
      case "登入":
      case "註冊":
        return !this.canActivate(null, null);
      case "登出":
        return this.canActivate(null, null);
      default:
        return true;
    }
  }

  public Login() {
    this.authChanged.next(true);
  }
  
  public Logout() {
    this.cookieService.delete(this.TOKEN_COOKIE_NAME);
    this.authChanged.next(true);
  }

  private set token(token: string) {
    this.cookieService.set(this.TOKEN_COOKIE_NAME, token)
  }

  private get token(): string {
    return this.cookieService.get(this.TOKEN_COOKIE_NAME);
  }
}
