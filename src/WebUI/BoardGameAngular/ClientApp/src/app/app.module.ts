import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RootRoutes } from './nav-menu/route.const';
import { LobbyComponent } from './lobby/lobby.component';
import { UserModule } from './user/user.module';
import { GuestService } from './auth/guest.service';
import { RoomModule } from './lobby/room/room.module';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    LobbyComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(
      RootRoutes,
      { useHash: true }),
    UserModule,
    RoomModule,
  ],
  providers: [
    GuestService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
