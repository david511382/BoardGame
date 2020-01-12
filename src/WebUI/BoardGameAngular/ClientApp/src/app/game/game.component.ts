import { Component, ViewChild, TemplateRef } from '@angular/core';
import { RoomService } from '../lobby/room/room.service';
import { LobbyService } from '../lobby/lobby.service';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameMainComponent{
  @ViewChild('Loading', { static: true }) LoadingPage: TemplateRef<any>;
  @ViewChild('BigTwo', { static: true }) BigTwoPage: TemplateRef<any>;

  public displayPage: TemplateRef<any>;
  public contextData = { $implicit: 'Loading...', game: 'game' };

  private gameID: number;

  constructor() {
    setTimeout(() => this.init(), 0);
  }

  private init() {
    this.displayPage = this.LoadingPage;
  }
}
