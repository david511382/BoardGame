import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import {  Observable  } from "rxjs";
import { catchError, tap } from 'rxjs/operators';
import { UserUrl, UrlConfigService } from '../config/config.service';
import { GeneralResponse,  HandleErrorFun, SuccessResponse } from '../domain/response.const';
import { AuthService } from '../auth/auth.service';

export class LoginRequest {
  constructor(public Username: string, public Password: string) {}
}

export class UserInfoRequest{
  constructor(public Name: string,public Username: string, public Password: string) { }
}

export class UserInfoModel {
  constructor(public Name: string, public Username: string) { }
}

interface LoginResponse extends GeneralResponse {
  name: string,
  username: string,
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class UserService {
  public get userInfo(): UserInfoModel {
    var dataBuf = this.authService.userDataBuffer;
    if (dataBuf)
      return new UserInfoModel(dataBuf.name, dataBuf.username);
    return null;
  }

  private readonly backendUrl: UserUrl;
  
  constructor(private http: HttpClient,
    config: UrlConfigService,
    private authService: AuthService) {
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
      tap(data => {
        this.authService.Login();

        if (this.authService.canActivate(null, null)) {
          this.updateUserInfo(data.name, data.username);
        }
      }
      ));
  }

  public RegisterAndLogin(request: UserInfoRequest): Observable<LoginResponse> {
    if (!request.Name || request.Name === "") {
      alert("請輸入名稱");
      return;
    }
    if (!request.Username || request.Username === "") {
      alert("請輸入使用者名稱");
      return;
    }
    if (!request.Password || request.Password === "") {
      alert("請輸入使用者密碼");
      return;
    }

    return this.http.post<LoginResponse>(
      this.backendUrl.Register,
      request
    ).pipe(
      catchError(HandleErrorFun()),
      tap(data => {
        this.authService.Login();

        if (this.authService.canActivate(null, null)) {
          this.updateUserInfo(data.name, data.username);
        }
      }
      ));
  }

  public Update(request: UserInfoRequest): Observable<SuccessResponse> {
    if (!request.Name || request.Name === "") {
      alert("請輸入名稱");
      return null;
    }
    if (!request.Username || request.Username === "") {
      alert("請輸入使用者名稱");
      return null;
    }
    
    return this.http.put<SuccessResponse>(
      this.backendUrl.Update,
      request
    ).pipe(catchError(HandleErrorFun()),
      tap(() => {
        if (this.authService.canActivate(null, null)) {
          this.updateUserInfo(request.Name, request.Username);
        }
      }));
  }

  public updateUserInfo(name: string, username: string) {
    this.authService.userDataBuffer.name = name;
    this.authService.userDataBuffer.username = username;
  }
}

