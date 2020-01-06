import { Injectable } from '@angular/core';
import { RoomModel } from '../room.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class GameRoomService {
  public roomData: RoomModel;
}
