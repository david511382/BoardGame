import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { RoomModule } from './room/room.module';
import { LobbyComponent } from './lobby.component';
import { UrlConfigService } from '../config/config.service';
import { LobbyService } from './lobby.service';
import { AuthService } from '../auth/auth.service';
import { ShareModule } from '../share/share.module';
import { RoomCreateComponent } from './room/create/create.component';
import { RoomListComponent } from './room/list/list.component';
import { RoomRoomComponent } from './room/room/room.component';

@NgModule({
  declarations: [
    LobbyComponent
  ],
  exports: [
    LobbyComponent,
    RoomListComponent,
    RoomCreateComponent,
    RoomRoomComponent,
  ],
  providers: [
    UrlConfigService,
    AuthService,
    LobbyService
  ],
  imports: [
    HttpModule,
    ShareModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule,
    RoomModule
  ],
  entryComponents: []
})
export class LobbyModule { }
