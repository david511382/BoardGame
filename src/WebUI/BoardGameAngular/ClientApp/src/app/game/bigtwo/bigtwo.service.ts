import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { UrlConfigService, BigTwoUrl } from '../../config/config.service';
import { HandleErrorFun, GeneralResponse } from '../../domain/response.const';
import { CardModel } from '../share/poker/poker-card/poker-card.component';
import { Observable } from 'rxjs';

interface CardResponseModel{
  number: number;
  suit: number;
}
interface HandCardsResponse extends GeneralResponse {
  cards: CardResponseModel[]
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
}

