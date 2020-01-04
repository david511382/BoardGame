import { Component, TemplateRef, ViewChild, Directive, Input } from '@angular/core';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css']
})
export class LobbyComponent{
  @ViewChild('ListRoom', { static: true }) listRoomPage: TemplateRef<any>;
  @ViewChild('CreateRoom', { static: true }) createRoomPage: TemplateRef<any>;
  @ViewChild('Room', { static: true }) roomPage: TemplateRef<any>;

  public displayPage: TemplateRef<any>;
  public contextData = { $implicit: 'World', game: 'final' };
  
  constructor() {
    setTimeout(() => this.GoListRoom(),0);
  }

  public GoCreateRoom() {
    this.displayPage = this.createRoomPage;
  }

  public GoListRoom() {
    this.displayPage = this.listRoomPage;
  }

  public GoRoom(roomData) {
    this.contextData.game = roomData;
    this.displayPage = this.roomPage;
  }
}
