import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RootRoutes } from './nav-menu/route.const';
import { UserModule } from './user/user.module';
import { GuestService } from './auth/guest.service';
import { GameModule } from './game/game.module';
import { LobbyService } from './lobby/lobby.service';
import { LobbyModule } from './lobby/lobby.module';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(
      RootRoutes,
      { useHash: true }),
    UserModule,
    LobbyModule,
    GameModule,
  ],
  providers: [
    GuestService,
    LobbyService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
