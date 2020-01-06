import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Observable  } from "rxjs";
import { catchError, tap } from 'rxjs/operators';
import { GeneralResponse, HandleErrorFun, SuccessResponse } from '../../domain/response.const';
import { UrlConfigService, RoomUrl, UserUrl } from '../../config/config.service';
import { GameModel } from './game.service';
import { AuthService } from '../../auth/auth.service';

export class UserModel {
  constructor(public id: number, public name: string, public username: string) { }
}

export class RoomModel {
  constructor(public hostID: number, public game: GameModel, public players: UserModel[]) { }
}

export class UserStatusModel{
  constructor(public room: RoomModel,
    public isInRoom: boolean,
    public isInGame: boolean) { }
}

interface ListResponse extends GeneralResponse {
  rooms: RoomModel[],
}

interface RoomResponse extends SuccessResponse {
  room: RoomModel,
}

interface UserStatusResponse extends GeneralResponse {
  room: RoomModel,
  isInRoom: boolean,
  isInGame: boolean,
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class RoomService {
  public readonly RoomPath: string = "gameroom";
  public readonly ListPath: string = "";
  public readonly CreatePath: string = "createroom";
  
  public RoomDataChanged = new EventEmitter<RoomModel>();
  public GameDataChanged = new EventEmitter<RoomModel>();

  public get GetRoomData(): RoomModel {
    return this.roomData;
  }

  private set setRoomData(data: RoomModel) {
    this.roomData = data;
    this.RoomDataChanged.emit(this.roomData);
  }

  private readonly backendUrl: RoomUrl
  private readonly userBackendUrl: UserUrl

  private roomData: RoomModel;

  constructor(private http: HttpClient, config: UrlConfigService, authService: AuthService) {
    this.backendUrl = config.roomBackendUrl;
    this.userBackendUrl = config.userBackendUrl;

    if (authService.canActivate(null,null))
      this.getUserStatus();
    else
      this.roomData = null;

    authService.authChanged.subscribe((isLogin) => {
      if (isLogin)
        this.getUserStatus();
      else
        this.roomData = null;
    });
  }
  
  public List(): Observable<ListResponse> {    
    return this.http.get<ListResponse>(this.backendUrl.List)
      .pipe(catchError(HandleErrorFun()));
  }

  public Create(gameID: number): Observable<RoomResponse> {
    let formData: FormData = new FormData();
    formData.append('gameID', gameID.toString());

    return this.http.post<RoomResponse>(
      this.backendUrl.Create,
      formData
    ).pipe(catchError(HandleErrorFun()),
      tap(resp => {
        if (resp.isError) {
          return;
        }

        if (resp.isSuccess)
          this.setRoomData = resp.room;
      }))
  }

  public Join(hostID: number): Observable<RoomResponse> {
    let formData: FormData = new FormData();
    formData.append('hostID', hostID.toString());

    return this.http.patch<RoomResponse>(
      this.backendUrl.Join,
      formData
    ).pipe(catchError(HandleErrorFun()),
      tap(resp => {
        if (resp.isError) {
          return;
        }

        if (resp.isSuccess)
          this.setRoomData = resp.room;
      }))
  }

  public Leave(): Observable<SuccessResponse> {
    return this.http.delete<SuccessResponse>(this.backendUrl.Join)
      .pipe(catchError(HandleErrorFun()),
        tap(resp => {
          if (resp.isError) {
            return;
          }

          if (resp.isSuccess)
            this.roomData = null;
        }))
  }

  public Start(): Observable<SuccessResponse> {
    return this.http.delete<SuccessResponse>(this.backendUrl.Start)
      .pipe(catchError(HandleErrorFun()),
        tap(resp => {
          if (resp.isError) {
            return;
          }

          if (resp.isSuccess)
            this.roomData = null;
        }))
  }

  private getUserStatus() {
    var ob = this.http.get<UserStatusResponse>(this.userBackendUrl.UserStatus)
      .pipe(catchError(HandleErrorFun()));
    if (ob)
      ob.subscribe(resp => {
        if (resp.isError) {
          return;
        }

        if (resp.isInRoom) {
          this.setRoomData = resp.room;
        } else if (resp.isInGame) {
          ///this.GameDataChanged.emit(this.RoomData);
          alert("is in game");
        }
      });
  }
}

