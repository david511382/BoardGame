import { Component, ViewChild, OnInit } from '@angular/core';
import { SetComponent } from '../../../share/set/set.component';
import { RoomService } from '../room.service';
import { GameRoomInfoComponent } from './game-room-info/game-room-info.component';
import { RoomPlayerComponent } from './player/player.component';
import { Router } from '@angular/router';
import { RoomSignalREventService } from '../signalr-event.service';
import { AuthService } from '../../../auth/auth.service';
import { RoomModel } from '../../../domain/user-status-model.const';

@Component({
  selector: 'app-lobby-room-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.css'],
})
export class RoomRoomComponent implements OnInit{
  @ViewChild(SetComponent, { static: true }) teamSet: SetComponent;
  @ViewChild(GameRoomInfoComponent, { static: true }) roomInfo: GameRoomInfoComponent;

  public readonly LobbyPath: string = this.service.ListPath;

  public get isHost() {
    return true;
  }
  
  constructor(private service: RoomService,
    private router: Router,
    authService: AuthService,
    signalService: RoomSignalREventService) {
    signalService.RoomPlayerChanged.subscribe((roomData: RoomModel) => this.show(roomData));
    signalService.RoomClose.subscribe(() => {
      this.service.isInRoom = false;
      //alert("房主關閉房間");
      this.goLobby();
    });
    signalService.RoomStarted.subscribe((gameId) => {
      authService.getUserStatus()//get game info
        .then(() => this.goGame());
    });
  }

  ngOnInit(): void {
    setTimeout(() => this.load(), 0);
  }

  private load() {
    if (!this.service.isInRoom) {
      this.goLobby();
      return;
    }

    var roomData = this.service.roomData;
    this.show(roomData)
  }

  private show(roomData: RoomModel) {
    this.roomInfo.Show(roomData);

    this.teamSet.Clear();
    var teamSet = this.teamSet;
    roomData.players.forEach((p) => {
      teamSet.Add(RoomPlayerComponent, p);
    });
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
    this.router.navigate(["game"]);
  }
}
