import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { UrlConfigService, BigTwoUrl } from '../../config/config.service';
import { HandleErrorFun, GeneralResponse, SuccessResponse } from '../../domain/response.const';
import { Observable } from 'rxjs';
import { BigtwoSignalREventService } from './bigtwo-signalr-event.service';

export class CardIndexesRequest {
  Indexes :number[]
}

export interface ICardResponseModel{
  number: number;
  suit: number;
}
interface IHandCardsResponse extends GeneralResponse {
  cards: ICardResponseModel[]
}
interface ICardIndexesResponse extends GeneralResponse {
  cardIndexes: number[]
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class BigTwoService {
  private readonly bigTwoBackendUrl: BigTwoUrl;

  constructor(private http: HttpClient,
    private signalService: BigtwoSignalREventService,
    config: UrlConfigService) {
    this.bigTwoBackendUrl = config.bigTwoBackendUrl;
  }

  public GetHandCards(): Observable<IHandCardsResponse> {
    return this.http.get<IHandCardsResponse>(this.bigTwoBackendUrl.HandCards)
      .pipe(catchError(HandleErrorFun()));
  }

  public SelectCards(request: CardIndexesRequest): Observable<ICardIndexesResponse> {
    return this.http.post<ICardIndexesResponse>(this.bigTwoBackendUrl.SelectCards, request)
      .pipe(catchError(HandleErrorFun()));
  }

  public PlayCards(request: CardIndexesRequest): Observable<SuccessResponse> {
    var option = {
      headers: new HttpHeaders()
        .append("cid", this.signalService.connectionId)
    };

    return this.http.post<SuccessResponse>(
      this.bigTwoBackendUrl.PlayCards,
      request,
      option
    ).pipe(catchError(HandleErrorFun()));
  }
}

