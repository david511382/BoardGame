import { EventEmitter, Injectable } from '@angular/core';
import { RoomModel } from './room.service';
import { SignalRService, HubEnum } from '../../share/services/signalR/signalr-manager.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class RoomSignalRService {
  public ConnectionId: string;

  RoomOpened = new EventEmitter();
  RoomPlayerChanged = new EventEmitter<RoomModel>();
  RoomStarted = new EventEmitter();
  RoomClose = new EventEmitter();
  
  constructor(private signalRService: SignalRService) {
    this.registerChannel();
  }
  
  private registerChannel() {
    var hubId = HubEnum.GameRoom;
    this.signalRService.RegisterOnServerEvents(hubId, 'SetConnectionId', (id) => {
      this.ConnectionId = id
    });
    this.signalRService.RegisterOnServerEvents(hubId,'RoomOpened', () => {
      this.RoomOpened.emit();
    });
    this.signalRService.RegisterOnServerEvents(hubId,'RoomPlayerChanged', (roomData: RoomModel) => {
      this.RoomPlayerChanged.emit(roomData);
    });
    this.signalRService.RegisterOnServerEvents(hubId,'RoomStarted', () => {
      this.RoomStarted.emit();
    });
    this.signalRService.RegisterOnServerEvents(hubId,'RoomClose', () => {
      this.RoomClose.emit();
    });
  }
}    
