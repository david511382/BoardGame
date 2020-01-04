import { Component, ViewChild, OnInit, EventEmitter, Output } from '@angular/core';
import { SetComponent } from '../../../share/set/set.component';
import { RoomService, RoomModel } from '../room.service';
import { GameRoomComponent } from './game-room/game-room.component';
import { GameRoomInfoComponent } from '../room/game-room-info/game-room-info.component';
import { AuthService } from '../../../auth/auth.service';

@Component({
  selector: 'app-lobby-room-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class RoomListComponent implements OnInit{
  @ViewChild(SetComponent, { static: true }) roomSet: SetComponent;
  @ViewChild(GameRoomInfoComponent, { static: true }) roomInfo: GameRoomInfoComponent;

  @Output() RedirectToCreateRoomEvent = new EventEmitter();
  @Output() RedirectToRoomEvent = new EventEmitter<RoomModel>();

  private isClickRoom: boolean;
  
  constructor(private service: RoomService, private authService: AuthService) {
    this.isClickRoom = false;
  }

  public CreateRoom() {
    if (!this.authService.canActivate(null, null)) {
      alert(`無法創建房間\n請先登入`);
      return;
    }

    this.RedirectToCreateRoomEvent.emit();
  }

  ngOnInit(): void {
    this.roomSet.ItemClickedEvent.subscribe((data) => this.clickRoom(data));
    this.roomSet.ItemDoubleClickedEvent.subscribe((data) => this.doubleClickRoom(data));
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

        var roomSet = this.roomSet;
        resp.rooms.forEach((room) => {
          roomSet.Add(GameRoomComponent, room);
        })
      });
  }

  private clickRoom(data: RoomModel) {
    this.isClickRoom = true;
    setTimeout(() => {
      if (this.isClickRoom) {
        this.isClickRoom = false;
        this.roomInfo.Show(data);        
      }}, 300);
  }

  private doubleClickRoom(data: RoomModel) {
    this.isClickRoom = false;
    if (!this.authService.canActivate(null, null)) {
      alert(`無法加入房間\n請先登入`);
      return;
    }

    this.joinRoom(data);
  }

  private joinRoom(data: RoomModel) {
    var ob = this.service.Join(data.hostID);
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        if (resp.isSuccess)
          this.RedirectToRoomEvent.emit(resp.room);
        else
          alert(resp.message);
      });
  }
}
