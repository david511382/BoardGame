import { NgModule } from '@angular/core';
import { CardComponent } from './poker-card/poker-card.component';
import { ShareModule } from '../../../share/share.module';
import { HandCardsComponent } from './hand-cards/hand-cards.component';
import { BrowserModule } from '@angular/platform-browser';

@NgModule({
  declarations: [
    CardComponent,
    HandCardsComponent,
  ],
  exports: [
    CardComponent,
    HandCardsComponent,
  ],
  providers: [
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    ShareModule,
  ],
  entryComponents: [
    CardComponent,
    HandCardsComponent,
  ]
})
export class PokerModule { }
