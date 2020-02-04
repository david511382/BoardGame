import { Component, Input, EventEmitter, Output, ElementRef } from '@angular/core';

export enum SuitEnum {
  S = 3,
  H = 2,
  D = 1,
  C = 0,
}

export class CardModel {
  public get Number(): number { return this._number;}
  public set number(value: number) {
    if (value < 1 || value > 13)
      return;

    this._number = value;
  }
  private _number: number;
  public get NumberText() {
    switch (this.Number) {
      case 1:
        return "A";
      case 11:
        return "J";
      case 12:
        return "Q";
      case 13:
        return "K";
      default:
        return this.Number;
    }
  }

  public get Suit(): SuitEnum { return this._suit as SuitEnum};
  public set suit(value: number) {
    if (value < SuitEnum.C || value > SuitEnum.S)
      return;

    this._suit = value;
  }
  private _suit: number;
  public get SuitText() {
    switch (this.Suit) {
      case SuitEnum.S:
        return "S";
      case SuitEnum.H:
        return "H";
      case SuitEnum.D:
        return "D";
      case SuitEnum.C:
        return "C";
    }
  }

  constructor(number?: number, suit?: number) {
    this.number = number;
    this.suit = suit;
  }
}

@Component({
  selector: 'game-share-poker-card',
  templateUrl: './poker-card.component.html',
  styleUrls: ['./poker-card.component.css']
})
export class CardComponent{
  @Input() data: CardModel;

  @Output() MouseEnterEvent = new EventEmitter<any>();
  @Output() MouseLeaveEvent = new EventEmitter<any>();
  @Output() MouseClickEvent = new EventEmitter<any>();
  @Output() DoubleClickEvent = new EventEmitter<any>();

  constructor(
    public Ref: ElementRef
  ) { }

  public get SuitColor() {
    switch (this.data.Suit) {
      case SuitEnum.S:
        return "black";
      case SuitEnum.H:
        return "red";
      case SuitEnum.D:
        return "orange";
      case SuitEnum.C:
        return "gray";
      default:
        return "white";
    }
  }
}
