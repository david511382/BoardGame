import { NgModule } from '@angular/core';
import { PokerModule } from './poker/poker.module';
import { CardComponent } from './poker/poker-card/poker-card.component';
import { HandCardsComponent } from './poker/hand-cards/hand-cards.component';

@NgModule({
  declarations: [
  ],
  exports: [
    CardComponent,
    HandCardsComponent,
  ],
  providers: [
  ],
  imports: [
    PokerModule
  ],
  entryComponents: [
  ]
})
export class GameShareModule { }
