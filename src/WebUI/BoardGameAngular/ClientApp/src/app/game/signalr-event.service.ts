import { Injectable } from '@angular/core';
import { RoomSignalRService } from '../share/services/signalR/room-signalr.service';

@Injectable()
export class GameSignalREventService {
  public connectionId: string;

  constructor(private signalRService: RoomSignalRService) {
    this.connectionId = this.signalRService.connectionId;
    this.signalRService.connectIdEvent.subscribe((id) => this.connectionId = id);
    this.registerChannel();
  }
  
  private registerChannel() {
  }
}    
