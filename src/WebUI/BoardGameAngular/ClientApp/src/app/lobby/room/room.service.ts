import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable  } from "rxjs";
import { catchError, tap } from 'rxjs/operators';
import { GeneralResponse, HandleErrorFun, SuccessResponse } from '../../domain/response.const';
import { UrlConfigService, RoomUrl } from '../../config/config.service';
import { GameModel } from './game.service';
import { RoomSignalREventService } from './signalr-event.service';
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

interface StartRoomResponse extends SuccessResponse {
  hostID: number;
  gameID: number;
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

  private readonly backendUrl: RoomUrl;

  private roomData: RoomModel;

  constructor(
    private http: HttpClient,
    private signalService: RoomSignalREventService,
    authService:AuthService,
    config: UrlConfigService) {
    this.backendUrl = config.roomBackendUrl;
    this.roomData = null;

    authService.authChanged.subscribe((isLogin) => {
      if (!isLogin)
        this.roomData = null;
    });

    signalService.RoomPlayerChanged.subscribe((roomData: RoomModel) => this.setRoomData(roomData));
    signalService.RoomClose.subscribe(() => this.setRoomData(null));
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

    var option = {
      headers: new HttpHeaders()
        .append('cid', this.signalService.connectionId)
    };

    return this.http.post<RoomResponse>(
      this.backendUrl.Create,
      formData,
      option
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

    var option = {
      headers: new HttpHeaders()
        .append('cid', this.signalService.connectionId)
    };

    return this.http.patch<RoomResponse>(
      this.backendUrl.Join,
      formData,
      option
    ).pipe(catchError(HandleErrorFun()),
      tap(resp => {
        if (resp.isError) {
          return;
        }

        if (resp.isSuccess)
          this.setRoomData(resp.room);
      }));
  }

  public Leave(): Observable<SuccessResponse> {
    var option = {
      headers: new HttpHeaders()
        .append('cid', this.signalService.connectionId)
    };

    return this.http.delete<SuccessResponse>(this.backendUrl.Join, option)
      .pipe(catchError(HandleErrorFun()),
        tap(resp => {
          if (resp.isError) {
            return;
          }

          if (resp.isSuccess)
            this.roomData = null;
        }));
  }

  public Start(): Observable<StartRoomResponse> {
    return this.http.delete<StartRoomResponse>(this.backendUrl.Start)
      .pipe(catchError(HandleErrorFun()),
        tap(resp => {
          if (resp.isError) {
            return;
          }

          if (resp.isSuccess)
            this.roomData = null;
        }));
  }
}

