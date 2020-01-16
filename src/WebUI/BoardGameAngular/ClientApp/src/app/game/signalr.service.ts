import { EventEmitter, Injectable } from '@angular/core';
import { SignalRService, HubEnum } from '../share/services/signalR/signalr-manager.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class GameSignalRService {
  public ConnectionId: string;

  constructor(private signalRService: SignalRService) {
    this.registerChannel();
  }
  
  private registerChannel() {
    var hubId = HubEnum.GameRoom;
    this.signalRService.RegisterOnServerEvents(hubId, 'SetConnectionId', (id) => {
      this.ConnectionId = id
    });
  }
}    
