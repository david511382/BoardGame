import { Component, ViewChild, OnInit } from '@angular/core';
import { SetComponent } from '../../../share/set/set.component';
import { RoomService } from '../room.service';
import { GameRoomInfoComponent } from './game-room-info/game-room-info.component';
import { RoomPlayerComponent } from './player/player.component';
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
  private readonly GamePath: string = "game";

  public get isHost() {
    return true;
  }

  constructor(private service: RoomService,
    private router: Router) {}

  ngOnInit(): void {
    setTimeout(() => {
      let roomData = this.service.GetRoomData;
      if (roomData === null) {
        this.router.navigate([this.LobbyPath]);
        return;
      }

      this.roomInfo.Show(roomData);

      var teamSet = this.teamSet;
      roomData.players.forEach((p) => {
        teamSet.Add(RoomPlayerComponent, p);
      })
    }, 0);
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

  public StartRoom() {
    var ob = this.service.Start();
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        if (resp.isSuccess) {
          this.router.navigate([this.GamePath]);
        }
        else
          alert(resp.message);
      });
  }
}
