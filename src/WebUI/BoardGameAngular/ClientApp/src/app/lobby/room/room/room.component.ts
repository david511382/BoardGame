import { Component, ViewChild, OnInit } from '@angular/core';
import { SetComponent } from '../../../share/set/set.component';
import { RoomService } from '../room.service';
import { GameRoomInfoComponent } from './game-room-info/game-room-info.component';
import { RoomPlayerComponent } from './player/player.component';
import { GameRoomService } from './room.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lobby-room-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css'],
})
export class RoomRoomComponent implements OnInit{
  @ViewChild(SetComponent, { static: true }) teamSet: SetComponent;
  @ViewChild(GameRoomInfoComponent, { static: true }) roomInfo: GameRoomInfoComponent;

  public readonly LobbyPath: string = this.service.ListPath;
  
  constructor(private service: RoomService,
    private dataService: GameRoomService,
    private router: Router) {}
  
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
          this.router.navigate([this.LobbyPath]);
        else
          alert(resp.message);
      });
  }

  private initRoomInfo() {
    setTimeout(() => this.roomInfo.Show(this.dataService.roomData),0);
  }

  private initTeam() {
    setTimeout(() => {
      var teamSet = this.teamSet;
      this.dataService.roomData.players.forEach((p) => {
        teamSet.Add(RoomPlayerComponent, p);
      })
    }, 0);
  }
}
