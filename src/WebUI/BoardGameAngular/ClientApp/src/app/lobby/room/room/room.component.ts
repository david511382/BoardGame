import { Component, ViewChild, ViewChildren, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { SetComponent } from '../../../share/set/set.component';
import { RoomService, RoomModel } from '../room.service';
import { GameRoomInfoComponent } from './game-room-info/game-room-info.component';
import { RoomPlayerComponent } from './player/player.component';

@Component({
  selector: 'app-lobby-room-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css']
})
export class RoomRoomComponent implements OnInit{
  @ViewChild(SetComponent, { static: true }) teamSet: SetComponent;
  @ViewChild(GameRoomInfoComponent, { static: true }) roomInfo: GameRoomInfoComponent;

  @Output() RedirectToListRoomEvent = new EventEmitter();
  @Input() data: RoomModel;
    
  constructor(private service: RoomService) {}
  
  ngOnInit(): void {
    this.initRoomInfo();
    this.initTeam();
  }

  public LeaveRoom() {
    var ob = this.service.Leave();
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        if (resp.isSuccess)
          this.RedirectToListRoomEvent.emit();
        else
          alert(resp.message);
      });
  }

  private initRoomInfo() {
    setTimeout(() => this.roomInfo.Show(this.data),0);
  }

  private initTeam() {
    setTimeout(() => {
      var teamSet = this.teamSet;
      this.data.players.forEach((p) => {
        teamSet.Add(RoomPlayerComponent, p);
      })
    }, 0);
  }
}
