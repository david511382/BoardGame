import { Component, EventEmitter, Output } from '@angular/core';
import { SetItemComponent } from '../../../../share/set/set-item.component';
import { GameModel } from '../../game.service';

@Component({
  selector: 'app-lobby-room-list-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements SetItemComponent{
  @Output() ClickEvent = new EventEmitter();
  @Output() DoubleClickEvent = new EventEmitter();

  public get game(): string {
    return this.gameData.name;
  }
  public get description(): string {
    return this.gameData.description;
  }
  
  private gameData: GameModel;

  constructor() {
    this.gameData = new GameModel(null, null, null, 0, null);
  }

  public Init(data: any) {
    this.gameData = data;
  }

  public Click() {
    this.ClickEvent.emit(this.gameData);
  }

  public DoubleClick() {
    this.DoubleClickEvent.emit(this.gameData);
  }
}
