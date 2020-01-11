import { Component } from '@angular/core';
import { RoomService } from './room/room.service';
import { Router } from '@angular/router';
import { LobbyService } from './lobby.service';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.css'],
})
export class LobbyComponent{
  constructor(service: LobbyService, roomService: RoomService,
    private router: Router) {
    service.RedirectNotify.subscribe((path) => this.router.navigate([path]));
  }
}
