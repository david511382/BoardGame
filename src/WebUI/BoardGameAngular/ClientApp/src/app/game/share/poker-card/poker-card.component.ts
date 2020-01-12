import { Component, Input } from '@angular/core';

@Component({
  selector: 'game-share-poker-card',
  templateUrl: './poker-card.component.html',
  styleUrls: ['./poker-card.component.css']
})
export class CardComponent{
  @Input() Number: number;
  @Input() Suit: number;

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

  public get SuitText() {
    switch (this.Suit) {
      case 3:
        return "S";
      case 2:
        return "H";
      case 1:
        return "D";
      case 0:
        return "C";
      default:
        return "ERROR";
    }
  }

  public get SuitColor() {
    switch (this.Suit) {
      case 3://S
        return "black";
      case 2://H
        return "red";
      case 1://D
        return "orange";
      case 0://C
        return "gray";
      default:
        return "white";
    }
  }

  constructor() {}
}
