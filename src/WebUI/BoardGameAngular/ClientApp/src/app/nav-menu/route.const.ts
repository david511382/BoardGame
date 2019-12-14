import { Route } from "@angular/router";
import { LobbyComponent } from "../lobby/lobby.component";
import { LoginComponent } from "../user/login/login.component";
import { AuthService } from "../auth/auth.service";
import { RegisterComponent } from "../user/register/register.component";

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
    name: "登入"
  },
  {
    path: 'register',
    component: RegisterComponent,
    name: "註冊"
  },
  {
    path: '', 
    component: LobbyComponent,
    canActivate: [AuthService], 
    name: "登出"
  },
  { path: '**', redirectTo: '' },
];
