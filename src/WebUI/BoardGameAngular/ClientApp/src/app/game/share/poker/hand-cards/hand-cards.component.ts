import { Component, Input, OnInit, ViewChildren, QueryList, AfterViewInit, Output, EventEmitter, Renderer2, ChangeDetectorRef } from '@angular/core';
import { CardComponent, CardModel } from '../poker-card/poker-card.component';
import { CdkDragRelease, CdkDragStart, CdkDragMove } from '@angular/cdk/drag-drop';

class DragData {
  // for reset position
  public dragPosition;
  public draggingCardData: CardModel;

  public endDrag() {
    this.dragPosition = { x: 0, y: 0 };
    this.draggingCardData = null;
  }

  constructor() {
    this.endDrag();
  }
}

class ViewCard {
  public card: CardModel;
  public left: number;
  public top: number;
  public isSelecting: boolean;

  constructor() {
    this.left = 0;
    this.top = 0;
    this.isSelecting = false;
  }
}

@Component({
  selector: 'game-share-poker-hand-cards',
  templateUrl: './hand-cards.component.html',
  styleUrls: ['./hand-cards.component.css'],
})
export class HandCardsComponent implements OnInit, AfterViewInit{
  @Input() cards: CardModel[];
  @ViewChildren("pokerCard") private pokerCardViews: QueryList<CardComponent>;
  
  @Output() dragStartedEvent = new EventEmitter<CdkDragStart>();
  @Output() dragMoveEvent = new EventEmitter < CdkDragMove<CardModel>>();
  @Output() playCardsEvent = new EventEmitter<number[]>();
  @Output() selectCardEvent = new EventEmitter<number[]>();

  public viewCards: ViewCard[];

  public dragData: DragData;
  private get isDragging(): boolean {
    return this.dragData.draggingCardData !== null;
  }

  private readonly CARD_WIDTH = 160;
  private readonly CARD_SHOW_WIDTH = 50;
  private readonly CARD_BORDER_WIDTH = 2;
  private readonly CARD_SELECTED_UP_TOP = -100;
  private get CARD_HIDE_WIDTH() {
    return this.CARD_WIDTH - this.CARD_SHOW_WIDTH - this.CARD_BORDER_WIDTH;
  }

  private readonly MAX_SELECTED_CARD= 6;

  private selectedIndexes: number[];
  private selectingIndexes: number[];
  
  constructor(
    private renderer: Renderer2,
    private changeDetector: ChangeDetectorRef
  ) {
    this.dragData = new DragData();
  }

  ngOnInit(): void {
    this.selectedIndexes = [];
    this.selectingIndexes = [];
    this.viewCards = [];

    if (this.cards)
      this.showCards(this.cards);

    this.dragStartedEvent.subscribe((e) => this.dragSelectingCardsStartEvent(e));
    this.dragMoveEvent.subscribe((e) => this.dragSelectingCardsMoveEvent(e));
  }

  ngAfterViewInit(): void {
    this.pokerCardViews.changes.subscribe((v) => {
      v.forEach((c, index) => {
        c.MouseEnterEvent.subscribe(() => this.onMouseOn(index));
        c.MouseLeaveEvent.subscribe(() => this.onMouseLeave(index));
        c.MouseClickEvent.subscribe(() => this.onMouseClick(index));
      });
    });
  }

  selectCards(cardIndexes: number[]) {
    this.selectingIndexes = [];

    this.viewCards.forEach((v,index) => {
      let card = this.viewCards[index];
      card.top = 0;
      card.isSelecting = false;
    });

    cardIndexes.forEach((index) => {
      let card = this.viewCards[index];
      card.top = this.CARD_SELECTED_UP_TOP;
      card.isSelecting = true;
      this.selectingIndexes.push(index);
    });
  }

  showCards(cards: CardModel[]) {
    this.viewCards = [];
    var vcs = this.viewCards;
    cards.forEach((c) => {
      var vc = new ViewCard();
      vc.card = c;
      vcs.push(vc);
    });

    this.setCardPosition(0);

    this.changeDetector.detectChanges();
  }

  private onMouseOn(index: number) {
    if (this.isDragging)
      return;

    const startReflashIndex = index + 1;
    if (startReflashIndex >= this.viewCards.length)
      return;

    this.viewCards[startReflashIndex].left += this.CARD_HIDE_WIDTH;
    this.setCardPosition(startReflashIndex);
  }

  private onMouseLeave(index: number) {
    if (this.isDragging)
      return;

    this.setCardPosition(index);
  }

  private onMouseClick(index: number) {
    if (this.selectedIndexes.length > 0) {
      let lastSelectedIndex = this.selectedIndexes[this.selectedIndexes.length - 1];
      if (lastSelectedIndex === index)
        return;

      if (this.selectedIndexes.length >= this.MAX_SELECTED_CARD)
        this.selectedIndexes.shift();          
    }

    this.selectedIndexes.push(index);
    this.selectCardEvent.emit(this.selectedIndexes);
  }

  private setCardPosition(startIndex: number) {
    if (startIndex >= this.viewCards.length)
      return;

    var left = this.viewCards[startIndex].left;
    this.viewCards.forEach((c, i) => {
      if (i <= startIndex)
        return;
      left += this.CARD_SHOW_WIDTH;
      c.left = left;
    });

    // after playing, need it
    this.changeDetector.detectChanges();
  }

  private dragSelectingCardsStartEvent(e: CdkDragStart<CardModel>) {
    this.dragData.draggingCardData = e.source.data;
  }

  public dragSelectingCardsMoveEvent(e: CdkDragMove<CardModel>) {
    var left = 0;
    var lefts = [];
    var shiftLeft = 0;
    var draggingCardLeft;
    this.selectingIndexes.forEach((i) => {
      let card = this.viewCards[i];
      if (card.card.Number === this.dragData.draggingCardData.Number &&
        card.card.Suit === this.dragData.draggingCardData.Suit) {
        draggingCardLeft = card.left;
        shiftLeft = left;
      }
      else
        lefts.push(left);
      left += this.CARD_WIDTH;
    });

    var viewArr = this.pokerCardViews.toArray();
    this.viewCards.forEach((card, index) => {
      if (!card.isSelecting)
        return;
      else if (card.card.Number === this.dragData.draggingCardData.Number &&
        card.card.Suit === this.dragData.draggingCardData.Suit) {
        return;
      }

      let left = e.distance.x + (lefts[0] - shiftLeft) - (card.left - draggingCardLeft);
      lefts.shift();
      this.renderer.setStyle(viewArr[index].Ref.nativeElement, 'transform', `translateX(${left}px) translateY(${e.distance.y}px)`);
    });
  }

  public useDraggingCards() {
    var newViewCards = [];
    this.viewCards.forEach((card) => {
      if (card.isSelecting)
        return;
      newViewCards.push(card.card);
    });

    this.selectingIndexes = [];
    this.selectedIndexes = [];

    this.showCards(newViewCards);

    this.dragData.endDrag();
  }
  public dragCardsCancel() {
    var viewArr = this.pokerCardViews.toArray();
    this.viewCards.forEach((card, index) => {
      if (!card.isSelecting)
        return;
      else if (card.card.Number === this.dragData.draggingCardData.Number &&
        card.card.Suit === this.dragData.draggingCardData.Suit) {
        return;
      }

      this.renderer.removeStyle(viewArr[index].Ref.nativeElement, "transform");
    });

    this.dragData.endDrag();
  }
  private dragSelectingCardsReleaseEvent(e: CdkDragRelease<CardModel>) {
    var cards = [];
    this.viewCards.forEach((card, index) => {
      if (!card.isSelecting)
        return;

      cards.push(index);
    });

    this.playCardsEvent.emit(cards);
  }
 
}
