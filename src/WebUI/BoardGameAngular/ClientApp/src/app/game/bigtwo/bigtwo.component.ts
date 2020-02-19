import { Component, OnInit, ViewChild, Input, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { CardModel } from '../share/poker/poker-card/poker-card.component';
import { BigTwoService, CardIndexesRequest } from './bigtwo.service';
import { HandCardsComponent} from '../share/poker/hand-cards/hand-cards.component';
import { GameBoardComponent, BorderColor } from './game-board/game-board.component';
import { BigtwoSignalREventService, ICardResponseModel, IConditionModel, IGameBoardModel } from './bigtwo-signalr-event.service';
import { IGameData } from '../game.service';
import { ToggleButtonComponent } from '../../share/toggle-button/toggle-button.component';

export interface IBigtwoData extends IGameData {
  tableData: ICardResponseModel[][];
  playerData: IPlayerModel[];
  condition: IConditionModel;
}
interface IPlayerModel {
  id: number;
  handCards: ICardResponseModel[];
}

@Component({
  selector: 'app-game-bigtwo',
  templateUrl: './bigtwo.component.html',
  styleUrls: ['./bigtwo.component.css'],
})
export class BigtwoComponent implements OnInit, AfterViewInit {
  @Input() gameData: IBigtwoData;

  @Output() onGameOver = new EventEmitter();

  @ViewChild(HandCardsComponent, { static: true }) private handCardView: HandCardsComponent;
  @ViewChild("gameBoard", { static: true }) private gameBoard: GameBoardComponent;
  @ViewChild("passButton", { static: true }) private passButton: ToggleButtonComponent;
  
  public cards: CardModel[];

  private currentPlayerIndex: number;
  private get currentPlayer(): IPlayerModel {
    return this.gameData.playerData[this.currentPlayerIndex];
  }
  private get isTurn(): boolean {
    return this.gameData.condition.turnId === this.currentPlayerIndex;
  }

  constructor(private service: BigTwoService,
    private signalService: BigtwoSignalREventService,) { }

  ngOnInit(): void {
    this.signalService.gameBoardUpdateEvent
      .subscribe((data: IGameBoardModel) => {
        this.gameBoard.putCards(data.cards);
        this.gameData.condition = data.condition;
        let isGameOver = this.gameData.condition.winPlayerId > 0;
        if (isGameOver) {
          this.passButton.setEnable(false);
          this.handCardView.enable = false;

          let player = this.gameData.playerData.find((v) => {
            if (v.id === this.gameData.condition.winPlayerId)
              return true;
            return false;
          }).id;
          setTimeout(() => this.onGameOver.emit(), 3000);          
          alert(`${player}獲勝\n3秒後關閉`);
        }
        else {
          this.setTurn();

          if (this.isTurn &&
            this.passButton.isActive)
            this.pass();
        }
      });
    this.load();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.gameData.tableData.forEach((cards) => {
        this.gameBoard.putCards(cards);
      });

      this.setTurn();
    }, 0);

    this.getHandCards();

    this.passButton.valueChange.subscribe((isActive: boolean) => {
      if (this.isTurn && isActive) {
        // close toggle
        this.passButton.setToggle(false);
        this.pass();
      }
    });
  }

  private setTurn() {
    var turnId = this.gameData.condition.turnId;
    if (turnId < this.currentPlayerIndex)
      turnId += this.gameData.playerData.length;
    turnId -= this.currentPlayerIndex;
    this.gameBoard.setTurn(turnId);
  }

  private getHandCards() {
    var handCards = [];
    this.currentPlayer.handCards.forEach((v) => {
      handCards.push(new CardModel(v.number, v.suit));
    });

    this.handCardView.showCards(handCards);
  }

  private load() {
    this.gameData.playerData.find((v, i) => {
      if (v.id === this.gameData.playerId) {
        this.currentPlayerIndex = i;
        return true;
      }
    });

    this.cards = [];
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
  
  private pass() {
    this.playCards(null);
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
