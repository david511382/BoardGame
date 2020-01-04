import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Observable  } from "rxjs";
import { catchError } from 'rxjs/operators';
import { GeneralResponse, HandleErrorFun, SuccessResponse } from '../../domain/response.const';
import { UrlConfigService, RoomUrl } from '../../config/config.service';
import { GameModel } from './game.service';

export class RoomModel {
  constructor(public hostID: number, public game: GameModel, public playerIDs: number[]) { }
}

interface ListResponse extends GeneralResponse {
  rooms: RoomModel[],
}

interface RoomResponse extends SuccessResponse {
  room: RoomModel,
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class RoomService {
  private readonly backendUrl: RoomUrl
  
  constructor(private http: HttpClient, config: UrlConfigService) {
    this.backendUrl = config.roomBackendUrl;
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
    ).pipe(catchError(HandleErrorFun()));
  }

  public Join(hostID: number): Observable<RoomResponse> {
    let formData: FormData = new FormData();
    formData.append('hostID', hostID.toString());

    return this.http.patch<RoomResponse>(
      this.backendUrl.Join,
      formData
    ).pipe(catchError(HandleErrorFun()));
  }

  public Leave(): Observable<SuccessResponse> {
    return this.http.delete<SuccessResponse>(this.backendUrl.Join)
      .pipe(catchError(HandleErrorFun()));
  }
}

