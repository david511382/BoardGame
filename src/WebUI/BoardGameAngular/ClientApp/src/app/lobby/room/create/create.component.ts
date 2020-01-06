import { Component, ViewChild, OnInit } from '@angular/core';
import { SetComponent } from '../../../share/set/set.component';
import { GameInfoComponent } from './gam-info/game-info.component';
import { GameModel, GameService } from '../game.service';
import { GameComponent } from './game/game.component';
import { RoomService } from '../room.service';
import { GameRoomService } from '../room/room.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lobby-room-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class RoomCreateComponent implements OnInit{
  @ViewChild(SetComponent, { static: true }) gameSet: SetComponent;
  @ViewChild(GameInfoComponent, { static: true }) gameInfo: GameInfoComponent;
  
  public readonly RoomPath: string = this.roomService.RoomPath;
  public readonly LobbyPath: string = this.roomService.ListPath;

  public get IsSelectedRoom(): boolean{
    return this.selectedGame === null;
  }

  private selectedGame: GameModel;

  constructor(private service: GameService,
    private roomService: RoomService,
    private roomDataService: GameRoomService,
    private router: Router) {
    this.selectedGame = null;
  }

  public CreateRoom() {
    var ob = this.roomService.Create(this.selectedGame.id);
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        if (resp.isSuccess) {
          this.roomDataService.roomData = resp.room;
          this.router.navigate([this.RoomPath]);
        }
        else
          alert(resp.message);
      });
  }

  public Cancel() {
    this.router.navigate([this.LobbyPath]);
  }

  ngOnInit(): void {
    this.gameSet.ItemClickedEvent.subscribe((data) => this.clickRoom(data));
    this.load();
  }

  private load() {
    var ob = this.service.List();
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        var gameSet = this.gameSet;
        resp.games.forEach((game) => {
          gameSet.Add(GameComponent, game);
        })
      });
  }

  private clickRoom(data: GameModel) {
    this.selectedGame = data;
    this.gameInfo.Show(this.selectedGame);
  }
}
