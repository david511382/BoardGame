import { Component, OnInit, ViewChild, ElementRef, Renderer2 } from '@angular/core';
import { CardModel } from '../share/poker/poker-card/poker-card.component';
import { BigTwoService, CardIndexesRequest } from './bigtwo.service';
import { HandCardsComponent} from '../share/poker/hand-cards/hand-cards.component';

enum GameBoardBorderColor {
  InitColor = "gray",
  InColor = "green",
  OutColor = "red",
}

@Component({
  selector: 'app-game-bigtwo',
  templateUrl: './bigtwo.component.html',
  styleUrls: ['./bigtwo.component.css'],
  providers: [BigTwoService]
})
export class BigtwoComponent implements OnInit {
  @ViewChild(HandCardsComponent, { static: true }) private handCardView: HandCardsComponent;
  @ViewChild("gameBoard", { static: true }) private gameBoard: ElementRef;

  public cards: CardModel[];

  constructor(private service: BigTwoService,
    private renderer: Renderer2) { }

  ngOnInit(): void {
    this.load();
    this.setGameBoardBoarderColor(GameBoardBorderColor.InitColor);
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

  public isMouseOnTable: boolean = false;
  private load() {
    this.cards = [];
    this.getHandCards();
    this.handCardView.selectCardEvent.subscribe((is) => this.selectCard(is));
    this.handCardView.dragStartedEvent.subscribe(() => {
      this.setGameBoardBoarderColor(GameBoardBorderColor.OutColor);
    });
    this.handCardView.dragMoveEvent.subscribe((e) => {
      var x = e.pointerPosition.x,y = e.pointerPosition.y;
      var gbEl = this.gameBoard.nativeElement;
      this.isMouseOnTable = x >= gbEl.offsetLeft && x <= gbEl.offsetLeft + gbEl.offsetWidth &&
        y >= gbEl.offsetTop && y <= gbEl.offsetTop + gbEl.offsetHeight;

      var gameBoardBorderColor = (this.isMouseOnTable) ? GameBoardBorderColor.InColor : GameBoardBorderColor.OutColor;
      this.setGameBoardBoarderColor(gameBoardBorderColor);
    });
    this.handCardView.playCardsEvent.subscribe((cardIndexes) => {
      if (this.isMouseOnTable)
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

        this.setGameBoardBoarderColor(GameBoardBorderColor.InitColor);
        this.handCardView.useDraggingCards();
      });
  }

  private setGameBoardBoarderColor(color: GameBoardBorderColor) {
    this.renderer.setStyle(this.gameBoard.nativeElement, "border-color", color);
  }
}
