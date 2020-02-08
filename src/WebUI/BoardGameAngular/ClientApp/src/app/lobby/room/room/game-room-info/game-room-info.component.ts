import { Component } from '@angular/core';
import { RoomModel } from '../../../../domain/user-status-model.const';

@Component({
  selector: 'app-lobby-room-list-game-room-info',
  templateUrl: './game-room-info.component.html',
  styleUrls: ['./game-room-info.component.css']
})
export class GameRoomInfoComponent{
  public get host() {
    return (this.isContainData) ?
      this.roomData.players.find((p) => p.id == this.roomData.hostID).name:
      null;
  }
  public get players() {
    if (!this.isContainData)
      return "無";

    if (this.roomData.players === null)
      return null;

    var result = this.roomData.players.map((v)=>v.name).join(', ');
    return result;
  }
  public get currentPlayersCount() {
    if (!this.isContainData ||
      this.roomData.players === null)
      return null;

    return this.roomData.players.length;
  }
  public get isContainData() {
    return this.roomData !== null;
  }

  public get game(): string {
    return (this.isContainData) ?
      this.gameData.name :
      null;
  }
  public get description(): string {
    return (this.isContainData) ?
      this.gameData.description :
      null;
  }
  public get maxPlayersCount() {
    return (this.isContainData) ?
      this.gameData.maxPlayerCount :
      "";
  }
  public get minPlayersCount() {
    return (this.isContainData) ?
      this.gameData.minPlayerCount :
      "";
  }

  private get gameData() {
    return (this.isContainData) ?
      this.roomData.game :
      null;
  }

  private roomData: RoomModel = null;

  public Show(room: RoomModel) {
    this.roomData = room;
  }
}
