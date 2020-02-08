import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { GameShareModule } from '../share/game-share.module';
import { BigtwoComponent } from './bigtwo.component';
import { GameBoardComponent } from './game-board/game-board.component';
import { BigTwoService } from './bigtwo.service';
import { BigtwoSignalREventService } from './bigtwo-signalr-event.service';
import { SignalRModule } from '../../signalR/signalR.module';


@NgModule({
  declarations: [
    BigtwoComponent,
    GameBoardComponent 
  ],
  exports: [
    BigtwoComponent
  ],
  providers: [
    BigTwoService,
    BigtwoSignalREventService
  ],
  imports: [
    HttpModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule,
    GameShareModule,
    SignalRModule
  ],
  entryComponents: [
  ]
})
export class BigtwoModule { }
