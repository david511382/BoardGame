import { Component } from '@angular/core';
import { GameModel } from '../../game.service';

@Component({
  selector: 'app-lobby-room-list-game-info',
  templateUrl: './game-info.component.html',
  styleUrls: ['./game-info.component.css']
})
export class GameInfoComponent{
  public get game(): string {
    return this.gameData.name;
  }
  public get description(): string {
    return this.gameData.description;
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
  public get isContainData() {
    return this.gameData !== null;
  }
  
  private gameData: GameModel = null;

  public Show(game: GameModel) {
    this.gameData = game;
  }
}
