import { Route } from "@angular/router";
import { LobbyComponent } from "../lobby/lobby.component";

export interface IRoute extends Route {
  name?:string
}

export const RootRoutes: IRoute[] = [
  { path: '', redirectTo: '/lobby', pathMatch: 'full' },
  { path: 'lobby', component: LobbyComponent, name: "大廳" },
];
