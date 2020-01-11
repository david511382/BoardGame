import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { RoomModel, RoomService } from './room/room.service';
import { GeneralResponse, HandleErrorFun } from '../domain/response.const';
import { UrlConfigService, UserUrl } from '../config/config.service';
import { AuthService } from '../auth/auth.service';

export class UserStatusModel{
  constructor(public room: RoomModel,
    public isInRoom: boolean,
    public isInGame: boolean) { }
}

interface UserStatusResponse extends GeneralResponse {
  room: RoomModel,
  isInRoom: boolean,
  isInGame: boolean,
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class LobbyService {
  public readonly GamePath: string = "game";
  public readonly RoomPath: string = this.roomService.RoomPath;

  public RedirectNotify= new EventEmitter<string>();

  private readonly userBackendUrl: UserUrl

  constructor(private http: HttpClient,
    config: UrlConfigService,
    authService: AuthService,
    private roomService: RoomService) {
    this.userBackendUrl = config.userBackendUrl;
    
    authService.authChanged.subscribe((isLogin) => this.authChanged(isLogin));
  }

  private authChanged(isLogin: boolean) {
    if (isLogin)
      this.getUserStatus();
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
          this.roomService.SetRoomData(resp.room);
          this.RedirectNotify.emit(this.RoomPath);
        } else if (resp.isInGame) {
          ///this.GameDataChanged.emit(this.RoomData);
          this.RedirectNotify.emit(this.GamePath);
        }
      });
  }
}

