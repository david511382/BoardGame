import { Component, ViewChild, OnInit } from '@angular/core';
import { SetComponent } from '../../../share/set/set.component';
import { RoomService } from '../room.service';
import { GameRoomInfoComponent } from './game-room-info/game-room-info.component';
import { RoomPlayerComponent } from './player/player.component';
import { Router } from '@angular/router';
import { RoomSignalRService } from '../signalr.service';
import { CommonDataService } from '../../../share/services/common-data/common-data.service';

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
    private router: Router,
    signalService: RoomSignalRService,
    dataService: CommonDataService) {
    service.RoomDataChanged.subscribe(() => this.load());
    signalService.RoomStarted.subscribe((gameId) => {
      dataService.Set("gameId", gameId);
      this.goGame();
    });
  }

  ngOnInit(): void {
    setTimeout(() => this.load(), 0);
  }

  private load() {
    let roomData = this.service.GetRoomData;
    if (roomData === null) {
      this.router.navigate([this.LobbyPath]);
      return;
    }

    this.roomInfo.Show(roomData);

    this.teamSet.Clear();
    var teamSet = this.teamSet;
    roomData.players.forEach((p) => {
      teamSet.Add(RoomPlayerComponent, p);
    })
  }

  public LeaveRoom() {
    this.service.Leave()
      .subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        if (resp.isSuccess)
          this.goLobby();
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

        if (!resp.isSuccess)
          alert(resp.message);
      });
  }

  private goLobby() {
    this.router.navigate([this.LobbyPath]);
  }

  private goGame() {
    this.router.navigate([this.GamePath]);
  }
}
