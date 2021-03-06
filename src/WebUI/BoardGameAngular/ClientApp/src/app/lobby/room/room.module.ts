import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { RoomListComponent } from './list/list.component';
import { ShareModule } from '../../share/share.module';
import { UrlConfigService } from '../../config/config.service';
import { RoomService } from './room.service';
import { GameRoomComponent } from './list/game-room/game-room.component';
import { GameRoomInfoComponent } from './room/game-room-info/game-room-info.component';
import { GameComponent } from './create/game/game.component';
import { GameInfoComponent } from './create/gam-info/game-info.component';
import { RoomCreateComponent } from './create/create.component';
import { RoomPlayerComponent } from './room/player/player.component';
import { RoomRoomComponent } from './room/room.component';
import { RoomSignalREventService } from './signalr-event.service';
import { AuthService } from '../../auth/auth.service';
import { SignalRModule } from '../../signalR/signalR.module';
import { GameListService } from './game-list.service';

@NgModule({
  declarations: [
    RoomListComponent,
    GameRoomComponent,
    GameRoomInfoComponent,
    GameComponent,
    GameInfoComponent,
    RoomCreateComponent,
    RoomPlayerComponent,
    RoomRoomComponent,
  ],
  exports: [
    RoomListComponent,
    RoomCreateComponent,
    RoomRoomComponent,
  ],
  providers: [
    UrlConfigService,
    RoomService,
    GameListService,
    RoomSignalREventService,
    AuthService
  ],
  imports: [
    HttpModule,
    ShareModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule,
    SignalRModule
  ],
  entryComponents: [
    GameRoomComponent,
    GameComponent,
    RoomPlayerComponent
  ]
})
export class RoomModule { }
