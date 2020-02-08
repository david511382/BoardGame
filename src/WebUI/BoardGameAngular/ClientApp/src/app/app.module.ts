import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RootRoutes } from './nav-menu/route.const';
import { UserModule } from './user/user.module';
import { GuestService } from './auth/guest.service';
import { GameModule } from './game/game.module';
import { LobbyModule } from './lobby/lobby.module';
import { AuthService } from './auth/auth.service';
import { SignalRModule } from './signalR/signalR.module';
import { RouteRoomGuardService } from './nav-menu/route-room-guard.service';
import { RouteGameGuardService } from './nav-menu/route-game-guard.service';

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
    SignalRModule
  ],
  providers: [
    GuestService,
    AuthService,
    RouteRoomGuardService,
    RouteGameGuardService,
    {
      provide: APP_INITIALIZER,
      useFactory: (authService: AuthService) => () => authService.getUserStatus(),
      deps: [AuthService],
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
