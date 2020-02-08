import { Component, OnInit, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { CardModel } from '../share/poker/poker-card/poker-card.component';
import { BigTwoService, CardIndexesRequest } from './bigtwo.service';
import { HandCardsComponent} from '../share/poker/hand-cards/hand-cards.component';
import { GameBoardComponent, BorderColor } from './game-board/game-board.component';
import { BigtwoSignalREventService } from './bigtwo-signalr-event.service';

@Component({
  selector: 'app-game-bigtwo',
  templateUrl: './bigtwo.component.html',
  styleUrls: ['./bigtwo.component.css'],
})
export class BigtwoComponent implements OnInit {
  @ViewChild(HandCardsComponent, { static: true }) private handCardView: HandCardsComponent;
  @ViewChild("gameBoard", { static: true }) private gameBoard: GameBoardComponent;

  public cards: CardModel[];

  constructor(private service: BigTwoService,
    private signalService: BigtwoSignalREventService,) { }

  ngOnInit(): void {
    this.signalService.gameBoardUpdateEvent
      .subscribe((data) => this.gameBoard.putCards(data));
    this.load();
  }

  private getHandCards() {
    var ob = this.service.GetHandCards();
    if (ob)
      ob.subscribe(resp => {
        if (resp.isError) {
          return;
        }

        var cards = [];
        resp.cards.forEach((v) => {
          cards.push(new CardModel(v.number, v.suit));
        });
        this.handCardView.showCards(cards);
      });
  }

  private load() {
    this.cards = [];
    this.getHandCards();
    this.handCardView.selectCardEvent.subscribe((is) => this.selectCard(is));
    this.handCardView.dragStartedEvent.subscribe(() => {
      this.gameBoard.setBoarderColor(BorderColor.OutColor);
    });
    this.handCardView.dragMoveEvent.subscribe((e) => {
      var x = e.pointerPosition.x, y = e.pointerPosition.y;
      this.gameBoard.dragOn(x,y);
    });
    this.handCardView.playCardsEvent.subscribe((cardIndexes) => {
      if (this.gameBoard.isMouseOnTable)
        this.playCards(cardIndexes);
      else
        this.handCardView.dragCardsCancel();
    });
  }

  private selectCard(cardIndexes: number[]) {
    var request: CardIndexesRequest = new CardIndexesRequest();
    request.Indexes = cardIndexes;
    this.service.SelectCards(request)
      .subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage);
          return;
        }

        let cardIndexes = resp.cardIndexes;
        if (!cardIndexes)
          return;

        this.handCardView.selectCards(cardIndexes);
      });
  }

  private playCards(cardIndexes: number[]) {
    var request: CardIndexesRequest = new CardIndexesRequest();
    request.Indexes = cardIndexes;
    this.service.PlayCards(request)
      .subscribe((resp) => {
        if (resp.isError) {
          this.handCardView.dragCardsCancel();
          alert(resp.errorMessage);
          return;
        }

        if (!resp.isSuccess) {
          this.handCardView.dragCardsCancel();
          alert(resp.message);
          return;
        }

        this.gameBoard.setBoarderColor(BorderColor.InitColor);
        this.handCardView.useDraggingCards();
      });
  }
}
