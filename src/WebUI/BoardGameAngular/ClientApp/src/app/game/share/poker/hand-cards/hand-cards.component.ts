import { Component, Input, OnInit, ViewChildren, QueryList, AfterViewInit, Output, EventEmitter } from '@angular/core';
import { CardComponent, CardModel } from '../poker-card/poker-card.component';

class ViewCard {
  public card: CardModel;
  public get leftCSS(): string {
    return `${this.left}px`;
  }
  public get topCSS(): string {
    return `${this.top}px`;
  }
  public left: number;
  public top: number;
  constructor() {
    this.left = 0;
    this.top = 0;
  }
}

@Component({
  selector: 'game-share-poker-hand-cards',
  templateUrl: './hand-cards.component.html',
  styleUrls: ['./hand-cards.component.css']
})
export class HandCardsComponent implements OnInit, AfterViewInit{
  @Input() cards: CardModel[];
  @ViewChildren("pokerCard") private pokerCardViews: QueryList<CardComponent>;

  @Output() ClickEvent = new EventEmitter<number>();

  public viewCards: ViewCard[];

  private readonly CARD_WIDTH = 160;
  private readonly CARD_SHOW_WIDTH = 50;
  private readonly CARD_BORDER_WIDTH = 2;
  private get CARD_HIDE_WIDTH() {
    return this.CARD_WIDTH - this.CARD_SHOW_WIDTH - this.CARD_BORDER_WIDTH;
  }

  ngAfterViewInit(): void {
  }

  ngOnInit(): void {
    this.viewCards = [];

    if (this.cards)
      this.ShowCards(this.cards);
  }

  ShowCards(cards: CardModel[]) {
    var vcs = this.viewCards;
    cards.forEach((c) => {
      var vc = new ViewCard();
      vc.card = c;
      vcs.push(vc);
    });

    this.setCardPosition(0);

    setTimeout(() => {
      this.pokerCardViews.forEach((c, index) => {
        c.MouseEnterEvent.subscribe((e) => this.onMouseOn(index));
        c.MouseLeaveEvent.subscribe((e) => this.onMouseLeave(index));
        c.ClickEvent.subscribe((e) => this.ClickEvent.emit(index));
      });
    }, 0);
  }

  private onMouseOn(index: number) {
    var startReflashIndex = index + 1;
    if (startReflashIndex >= this.viewCards.length)
      return;

    this.viewCards[startReflashIndex].left += this.CARD_HIDE_WIDTH;
    this.setCardPosition(startReflashIndex);
  }

  private onMouseLeave(index: number) {
    this.setCardPosition(index);
  }

  private onDrag(index: number) {

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
  }
}
