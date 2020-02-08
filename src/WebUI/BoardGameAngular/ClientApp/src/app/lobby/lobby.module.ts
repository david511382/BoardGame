import { NgModule } from '@angular/core';
import { RoomModule } from './room/room.module';
import { LobbyComponent } from './lobby.component';

@NgModule({
  declarations: [
    LobbyComponent
  ],
  exports: [
    LobbyComponent
  ],
  providers: [
  ],
  imports: [
    RoomModule
  ],
  entryComponents: []
})
export class LobbyModule { }
