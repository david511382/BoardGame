import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders} from '@angular/common/http';
import { Observable  } from "rxjs";
import { catchError, tap } from 'rxjs/operators';
import { GeneralResponse, HandleErrorFun, SuccessResponse } from '../../domain/response.const';
import { UrlConfigService, RoomUrl } from '../../config/config.service';
import { RoomSignalREventService } from './signalr-event.service';
import { AuthService } from '../../auth/auth.service';
import { RoomModel } from '../../domain/user-status-model.const';

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
    return this.authService.userDataBuffer.room;
  }

  private setRoomData(data: RoomModel) {
    this.authService.userDataBuffer.room = data;
    this.RoomDataChanged.emit(data);
  }

  private readonly backendUrl: RoomUrl;

  constructor(
    private http: HttpClient,
    private signalService: RoomSignalREventService,
    private authService:AuthService,
    config: UrlConfigService) {
    this.backendUrl = config.roomBackendUrl;

    authService.userStatusDataChanged.subscribe((data) => this.SetRoomData(data.room));
    if (authService.userDataBuffer) {
      this.SetRoomData(this.authService.userDataBuffer.room);
    }

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
      }));
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
            this.setRoomData(null);
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
            this.setRoomData(null);
        }));
  }
}

