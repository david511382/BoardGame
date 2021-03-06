import { Component, AfterViewInit, Renderer2, ViewChild, ElementRef } from '@angular/core';
import { HandCardsComponent } from '../../share/poker/hand-cards/hand-cards.component';
import { CardModel } from '../../share/poker/poker-card/poker-card.component';
import { ICardResponseModel } from '../bigtwo-signalr-event.service';

export enum BorderColor {
  InitColor = "gray",
  InColor = "green",
  OutColor = "red",
}

@Component({
  selector: 'bigtwo-game-board',
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.css'],
})
export class GameBoardComponent implements AfterViewInit{
  @ViewChild("gameBoard", { static: true }) public board: ElementRef;
  @ViewChild("lastCards") private lastCardsView: HandCardsComponent;

  public isMouseOnTable: boolean = false;
  public arrowClass: string;

  private allCards: CardModel[][]=[];

  constructor(private renderer: Renderer2) {}

  ngAfterViewInit(): void {
    this.setBoarderColor(BorderColor.InitColor);
  }

  public setTurn(id: number) {
    switch (id) {
      case 0:
        this.arrowClass = "arrow bottom";
        break;
      case 1:
        this.arrowClass = "arrow right";
        break;
      case 2:
        this.arrowClass = "arrow top";
        break;
      case 3:
        this.arrowClass = "arrow left";
        break;
    }
  }

  public putCards(data:ICardResponseModel[]) {
    var cards = [];
    data.forEach((v) => {
      cards.push(new CardModel(v.number, v.suit));
    });
    this.allCards.push(cards);

    this.lastCardsView.showCards(cards);
  }

  public dragOn(x,y) {
    var gbEl = this.board.nativeElement;
    this.isMouseOnTable = x >= gbEl.offsetLeft && x <= gbEl.offsetLeft + gbEl.offsetWidth &&
      y >= gbEl.offsetTop && y <= gbEl.offsetTop + gbEl.offsetHeight;

    var gameBoardBorderColor = (this.isMouseOnTable) ? BorderColor.InColor : BorderColor.OutColor;
    this.setBoarderColor(gameBoardBorderColor);
  }

  public setBoarderColor(color: BorderColor) {
    this.renderer.setStyle(this.board.nativeElement, "border-color", color);
  }
}
