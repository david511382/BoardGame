import { Component, EventEmitter, Output } from '@angular/core';
import { SetItemComponent } from '../../../../share/set/set-item.component';

@Component({
  selector: 'app-lobby-room-room-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class RoomPlayerComponent implements SetItemComponent{
  @Output() ClickEvent = new EventEmitter();
  @Output() DoubleClickEvent = new EventEmitter();

  public player: number;
  public team: number;

  public get color(): string {
    switch (this.team) {
      case 1:
        return "crimson";
      case 2:
        return "blue";
      case 3:
        return "lightgreen";
      default:
        return "gray";
    }
  }

  constructor() {
    this.player = 0;
    this.team = 0;
  }

  public Init(player: number) {
    this.player = player;
    this.team = 1;
  }
}
