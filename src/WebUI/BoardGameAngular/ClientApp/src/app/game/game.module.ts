import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { BigtwoComponent } from './bigtwo/bigtwo.component';
import { GameMainComponent } from './game.component';
import { GameShareModule } from './share/game-share.module';

@NgModule({
  declarations: [
    GameMainComponent,
    BigtwoComponent,
  ],
  exports: [
  ],
  providers: [
  ],
  imports: [
    HttpModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule,
    GameShareModule
  ],
  entryComponents: [
  ]
})
export class GameModule { }
