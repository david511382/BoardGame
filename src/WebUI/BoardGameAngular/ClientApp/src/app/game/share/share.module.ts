import { NgModule } from '@angular/core';
import { CardComponent } from './poker-card/poker-card.component';

@NgModule({
  declarations: [
    CardComponent
  ],
  exports: [
    CardComponent,
  ],
  providers: [
  ],
  imports: [
  ],
  entryComponents: [
    CardComponent,
  ]
})
export class ShareModule { }
