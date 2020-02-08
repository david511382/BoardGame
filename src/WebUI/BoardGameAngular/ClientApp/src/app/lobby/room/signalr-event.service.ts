import { EventEmitter, Injectable } from '@angular/core';
import { RoomModel } from '../../domain/user-status-model.const';
import { RoomSignalRService } from '../../signalR/room-signalr.service';

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
