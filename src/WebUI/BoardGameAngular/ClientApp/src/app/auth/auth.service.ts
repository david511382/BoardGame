import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root',//singleton 
})
export class AuthService implements CanActivate {
  private readonly TOKEN_COOKIE_NAME: string = "token";

  constructor(private cookieService: CookieService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!this.token || this.token==="") {
      return false;
    }
    return true;
  }

  isNavShow(navName : string) :boolean {
    switch (navName) {
      case "登入":
        return !this.canActivate(null, null);
      case "登出":
        return this.canActivate(null, null);
      default:
        return true;
    }
  }

  public Login(token: string) {
    this.token = token;
  }

  public Logout() {
    this.token = "";
  }

  private set token(token: string) {
    this.cookieService.set(this.TOKEN_COOKIE_NAME, token)
  }

  private get token(): string {
    return this.cookieService.get(this.TOKEN_COOKIE_NAME);
  }
}
