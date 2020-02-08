import { Injectable, EventEmitter, Injector} from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserUrl, UrlConfigService } from '../config/config.service';
import { HttpClient } from '@angular/common/http';
import { GeneralResponse, HandleErrorFun } from '../domain/response.const';
import { catchError, tap } from 'rxjs/operators';
import { RoomModel } from '../domain/user-status-model.const';

export class UserStatusData {
  id: number;
  name: string;
  username: string;
  room: RoomModel;
  isInRoom: boolean;
  isInGame: boolean;

  constructor() {
    this.room = null;
  }
}

interface IUserStatusResponse extends GeneralResponse {
  id: number,
  name: string,
  username: string,
  room: RoomModel,
  isInRoom: boolean,
  isInGame: boolean,
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class AuthService implements CanActivate {
  public authChanged: BehaviorSubject<boolean> = new BehaviorSubject(false);
  public userStatusDataChanged = new EventEmitter<UserStatusData>();

  public userDataBuffer = new UserStatusData();

  private readonly backendUrl: UserUrl;

  private readonly TOKEN_COOKIE_NAME: string = "token";

  // inject Router on constructor not work in APP_INITIALIZER
  private get router(): Router { //this creates router property on your service.
    return this.injector.get(Router);
  }

  constructor(config: UrlConfigService,
    private http: HttpClient,
    private cookieService: CookieService,
    private injector: Injector) {
    this.backendUrl = config.userBackendUrl;
    this.authChanged.subscribe((isLogin) => {
      if (isLogin)
        this.getUserStatus();
      else
        this.userDataBuffer = new UserStatusData();
    });
  }

  public getUserStatus(): Promise<IUserStatusResponse> {
    return this.http.get<IUserStatusResponse>(this.backendUrl.getStatus)
      .pipe(catchError(HandleErrorFun()),
        tap(resp => {
          if (resp.isError) {
            return;
          }
          this.userDataBuffer.name = resp.name;
          this.userDataBuffer.username = resp.username;
          this.userDataBuffer.room= resp.room;
          this.userDataBuffer.id= resp.id;
          this.userDataBuffer.isInGame= resp.isInGame;
          this.userDataBuffer.isInRoom= resp.isInRoom;

          this.userStatusDataChanged.emit(this.userDataBuffer);
        })).toPromise();
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (!this.isLogin())
      return false;
    if (!state)
      return true;

    switch (state.url) {
      case "/gameroom":
        if (this.userDataBuffer.isInGame)
          return this.router.parseUrl('/game');
        else if (!this.userDataBuffer.isInRoom)
          return this.router.parseUrl('/lobby');
      case "/lobby":
        if (this.userDataBuffer.isInGame)
          return this.router.parseUrl('/game');
        else if (this.userDataBuffer.isInRoom)
          return this.router.parseUrl('/gameroom');
      default:
        return true;
    }
  }

  isNavShow(navName : string) :boolean {
    switch (navName) {
      case "登入":
      case "註冊":
        return !this.isLogin();
      case "登出":
      case "用戶資料":
        return this.isLogin();
      case "navHide":
        return false;
      default:
        return true;
    }
  }

  private isLogin() {
    if (!this.token || this.token === "") {
      return false;
    }
    return true;
  }

  public Login() {
    this.authChanged.next(true);
  }
  
  public Logout() {
    this.cookieService.delete(this.TOKEN_COOKIE_NAME);
    this.authChanged.next(false);
  }

  private set token(token: string) {
    this.cookieService.set(this.TOKEN_COOKIE_NAME, token);
  }

  private get token(): string {
    return this.cookieService.get(this.TOKEN_COOKIE_NAME);
  }
}
