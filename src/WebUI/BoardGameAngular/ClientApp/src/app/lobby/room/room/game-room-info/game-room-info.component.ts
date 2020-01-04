import { Component } from '@angular/core';
import { RoomModel } from '../../room.service';

@Component({
  selector: 'app-lobby-room-list-game-room-info',
  templateUrl: './game-room-info.component.html',
  styleUrls: ['./game-room-info.component.css']
})
export class GameRoomInfoComponent{
  public get host() {
    return (this.isContainData) ?
      this.roomData.hostID:
      null;
  }
  public get players() {
    if (!this.isContainData)
      return "無";

    if (this.roomData.playerIDs === null)
      return null;

    var result = this.roomData.playerIDs.join(', ');
    return result;
  }
  public get currentPlayersCount() {
    if (!this.isContainData ||
      this.roomData.playerIDs === null)
      return null;

    return this.roomData.playerIDs.length;
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
