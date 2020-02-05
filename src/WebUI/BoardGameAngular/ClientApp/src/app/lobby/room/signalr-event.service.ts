import { EventEmitter, Injectable } from '@angular/core';
import { RoomModel } from './room.service';
import { RoomSignalRService } from '../../share/services/signalR/room-signalr.service';

@Injectable()
export class RoomSignalREventService {
  public connectionId: string;

  RoomOpened = new EventEmitter();
  RoomPlayerChanged = new EventEmitter<RoomModel>();
  RoomStarted = new EventEmitter<number>();
  RoomClose = new EventEmitter();
  
  constructor(private signalRService: RoomSignalRService) {
    this.connectionId = this.signalRService.connectionId;
    this.signalRService.connectIdEvent.subscribe((id) => this.connectionId = id);
    this.registerChannel();
  }

  public goInGroup(roomId: string) {
    this.signalRService.send("GoInGroup", roomId);
  }

  private registerChannel() {
    this.signalRService.registerChannel('RoomOpened', () => {
      this.RoomOpened.emit();
    });
    this.signalRService.registerChannel('RoomPlayerChanged', (roomData: RoomModel) => {
      this.RoomPlayerChanged.emit(roomData);
    });
    this.signalRService.registerChannel('RoomStarted', (gameId) => {
      this.RoomStarted.emit(gameId);
    });
    this.signalRService.registerChannel('RoomClose', () => {
      this.RoomClose.emit();
    });
  }
}    
