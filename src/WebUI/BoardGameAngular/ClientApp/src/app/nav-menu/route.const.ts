import { Route } from "@angular/router";
import { LobbyComponent } from "../lobby/lobby.component";
import { LoginComponent } from "../user/login/login.component";
import { AuthService } from "../auth/auth.service";
import { RegisterComponent } from "../user/register/register.component";
import { UpdateComponent } from "../user/update/update.component";
import { GuestService } from "../auth/guest.service";
import { RoomRoomComponent } from "../lobby/room/room/room.component";
import { RoomCreateComponent } from "../lobby/room/create/create.component";
import { GameMainComponent } from "../game/game.component";

export interface IRoute extends Route {
  name?: string
  onNav?: boolean
}

export const RootRoutes: IRoute[] = [
  { path: '', redirectTo: '/lobby', pathMatch: 'full' },
  { path: 'lobby', component: LobbyComponent, name: "大廳" },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [GuestService],
    name: "登入"
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [GuestService],
    name: "註冊"
  },
  {
    path: 'update',
    component: UpdateComponent,
    canActivate: [AuthService],
    name: "用戶資料"
  },
  {
    path: '', 
    component: LobbyComponent,
    canActivate: [AuthService], 
    name: "登出"
  },
  {
    path: 'createroom',
    component: RoomCreateComponent,
    canActivate: [AuthService],
    name: "navHide"
  },
  {
    path: 'gameroom',
    component: RoomRoomComponent,
    canActivate: [AuthService],
    name: "navHide"
  },
  {
    path: 'game',
    component: GameMainComponent,
    canActivate: [AuthService],
    name: "navHide"
  },
  { path: '**', redirectTo: '' },
];
