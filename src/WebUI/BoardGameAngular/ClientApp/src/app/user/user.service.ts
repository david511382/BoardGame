import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import {  Observable  } from "rxjs";
import { catchError, tap } from 'rxjs/operators';
import { UserUrl, UrlConfigService } from '../config/config.service';
import { GeneralResponse,  HandleErrorFun } from '../domain/response.const';
import { AuthService } from '../auth/auth.service';

export class LoginRequest {
  constructor(public Username: string, public Password: string) {}
}

interface LoginResponse extends GeneralResponse {
  name: string,
}

@Injectable()
export class UserService {
  readonly backendUrl: UserUrl

  constructor(private http: HttpClient, config: UrlConfigService, private authService: AuthService) {
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

    let formData: FormData = new FormData();
    formData.append('username', request.Username);
    formData.append('password', request.Password);

    return this.http.post<LoginResponse>(
      this.backendUrl.Login,
      formData
    ).pipe(
      catchError(HandleErrorFun()),
      tap(() => this.authService.Login()
      ));
  }

  public Register() {
  }
}
