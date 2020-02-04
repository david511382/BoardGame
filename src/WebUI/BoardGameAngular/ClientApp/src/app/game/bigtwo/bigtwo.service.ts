import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { UrlConfigService, BigTwoUrl } from '../../config/config.service';
import { HandleErrorFun, GeneralResponse, SuccessResponse } from '../../domain/response.const';
import { Observable } from 'rxjs';

export class CardIndexesRequest {
  Indexes :number[]
}

interface CardResponseModel{
  number: number;
  suit: number;
}
interface HandCardsResponse extends GeneralResponse {
  cards: CardResponseModel[]
}
interface CardIndexesResponse extends GeneralResponse {
  cardIndexes: number[]
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class BigTwoService {
  private readonly bigTwoBackendUrl: BigTwoUrl

  constructor(private http: HttpClient,
    config: UrlConfigService) {
    this.bigTwoBackendUrl = config.bigTwoBackendUrl;
  }

  public GetHandCards(): Observable<HandCardsResponse> {
    return this.http.get<HandCardsResponse>(this.bigTwoBackendUrl.HandCards)
      .pipe(catchError(HandleErrorFun()));
  }

  public SelectCards(request: CardIndexesRequest ): Observable<CardIndexesResponse> {
    return this.http.post<CardIndexesResponse>(this.bigTwoBackendUrl.SelectCards, request)
      .pipe(catchError(HandleErrorFun()));
  }

  public PlayCards(request: CardIndexesRequest): Observable<SuccessResponse> {
    return this.http.post<SuccessResponse>(this.bigTwoBackendUrl.PlayCards, request)
      .pipe(catchError(HandleErrorFun()));
  }
}

