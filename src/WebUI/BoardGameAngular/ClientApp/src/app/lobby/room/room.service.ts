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

  public get GetRoomData(): RoomModel {
    return this.roomData;
  }

  private setRoomData(data: RoomModel) {
    this.roomData = data;
    this.RoomDataChanged.emit(this.roomData);
  }

  private readonly backendUrl: RoomUrl

  private roomData: RoomModel;

  constructor(private http: HttpClient, config: UrlConfigService) {
    this.backendUrl = config.roomBackendUrl;
    this.roomData = null;
  }

  public SetRoomData(roomData: RoomModel) {
    this.setRoomData(roomData);
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
          this.setRoomData(resp.room);
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
          this.setRoomData(resp.room);
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
}

