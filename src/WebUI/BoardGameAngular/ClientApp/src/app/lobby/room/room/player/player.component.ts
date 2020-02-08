import { Component, EventEmitter, Output } from '@angular/core';
import { SetItemComponent } from '../../../../share/set/set-item.component';
import { UserModel } from '../../../../domain/user-status-model.const';

@Component({
  selector: 'app-lobby-room-room-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class RoomPlayerComponent implements SetItemComponent{
  @Output() ClickEvent = new EventEmitter();
  @Output() DoubleClickEvent = new EventEmitter();

  public name: string;
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

  public Init(info: UserModel) {
    this.name = info.name;
    this.team = 1;
  }
}
