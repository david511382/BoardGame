import { Component, EventEmitter, Output } from '@angular/core';
import { RoomModel } from '../../room.service';
import { SetItemComponent } from '../../../../share/set/set-item.component';
import { GameModel } from '../../game.service';

@Component({
  selector: 'app-lobby-room-list-game-room',
  templateUrl: './game-room.component.html',
  styleUrls: ['./game-room.component.css']
})
export class GameRoomComponent implements SetItemComponent{
  @Output() ClickEvent = new EventEmitter();
  @Output() DoubleClickEvent = new EventEmitter();

  public maxPlayersCount: number;

  public get host(): string {
    return this.roomData.hostID.toString();
  }
  public get game(): string {
    return this.roomData.game.name;
  }
  public get currentPlayersCount(): number {
    return this.roomData.players.length;
  }
  public get roomColor(): string {
    return this.isFull ?
      "gray" :
      "lightgreen";
  }
  public get isFull(): boolean {
    return this.currentPlayersCount === this.maxPlayersCount;
  }
  
  private roomData: RoomModel;

  constructor() {
    this.roomData = new RoomModel(null, new GameModel(null, null, null, 0, null), []);
  }

  public Init(data: any) {
    this.roomData = data;
    this.maxPlayersCount = this.roomData.game.maxPlayerCount;
  }

  public Click() {
    this.ClickEvent.emit(this.roomData);
  }

  public DoubleClick() {
    this.DoubleClickEvent.emit(this.roomData);
  }
}
