import { Component, OnInit, ViewChild } from '@angular/core';
import { CardModel } from '../share/poker/poker-card/poker-card.component';
import { BigTwoService } from './bigtwo.service';
import { HandCardsComponent } from '../share/poker/hand-cards/hand-cards.component';

@Component({
  selector: 'app-game-bigtwo',
  templateUrl: './bigtwo.component.html',
  styleUrls: ['./bigtwo.component.css'],
  providers: [BigTwoService]
})
export class BigtwoComponent implements OnInit {
  @ViewChild(HandCardsComponent, { static: true }) private handCardView: HandCardsComponent;
  public cards: CardModel[];

  constructor(private service: BigTwoService) { }

  ngOnInit(): void {
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
        this.handCardView.ShowCards(cards);
      });
  }

  private load() {
    this.cards = [];
    this.getHandCards();
  }
}
