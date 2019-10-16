import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import {  Observable  } from "rxjs";
import { catchError } from 'rxjs/operators';
import { UserUrl, UrlConfigService } from '../config/config.service';
import { GeneralResponse,  HandleErrorFun } from '../domain/response.const';
import { CookieService } from 'ngx-cookie-service';

export class LoginRequest {
  constructor(public Username: string, public Password: string) {}
}

interface LoginResponse extends GeneralResponse {
  token: string,
  name: string,
}

@Injectable()
export class UserService {
  private readonly TOKEN_COOKIE_NAME:string = "token";
  readonly backendUrl: UserUrl

  constructor(private http: HttpClient, config: UrlConfigService, private cookieService: CookieService) {
    this.backendUrl = config.userBackendUrl;
  }

  public Login(request: LoginRequest): Observable<LoginResponse> {
    if (!request.Username || request.Username === "") {
      alert("請輸入使用者名稱");
      return;
    }
    if (!request.Password || request.Password === "") {
      alert("請輸入使用者密碼");
      return;
    }
    var t = this.Token;
    let formData: FormData = new FormData();
    formData.append('username', request.Username);
    formData.append('password', request.Password);

    return this.http.post<LoginResponse>(
      this.backendUrl.Login,
      formData)
      .pipe(catchError(HandleErrorFun()));
  }

  public Register() {
  }

  public set Token(token: string) {
    this.cookieService.set(this.TOKEN_COOKIE_NAME,token)
  }

  public get Token(): string {
    return this.cookieService.get(this.TOKEN_COOKIE_NAME);
  }
}
