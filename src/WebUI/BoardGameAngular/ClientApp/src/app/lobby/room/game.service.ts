import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Observable  } from "rxjs";
import { catchError } from 'rxjs/operators';
import { GeneralResponse, HandleErrorFun } from '../../domain/response.const';
import { UrlConfigService, GameUrl } from '../../config/config.service';
import { GameModel } from '../../domain/user-status-model.const';

interface ListResponse extends GeneralResponse {
  games: GameModel[],
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class GameService {
  private readonly backendUrl: GameUrl;
  
  constructor(private http: HttpClient, config: UrlConfigService) {
    this.backendUrl = config.gameBackendUrl;
  }
  
  public List(): Observable<ListResponse> {    
    return this.http.get<ListResponse>(this.backendUrl.List)
      .pipe(catchError(HandleErrorFun()));
  }
}

