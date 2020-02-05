import { Injectable, EventEmitter } from '@angular/core';
import { SignalRService, HubEnum } from './signalr-manager.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class RoomSignalRService {
  public connectIdEvent = new EventEmitter<string>();

  public connectionId: string;
  
  private hubId = HubEnum.GameRoom;

  constructor(private signalRService: SignalRService) {
    this.signalRService.create(HubEnum.GameRoom,
      "/gameRoomHub",
      (hub) => hub.Send("GetConnectionId"));
    this.connectIdEvent.subscribe((id) => this.connectionId = id);
    this.init();
  }

  public registerChannel(channel:string, handler: (...args: any[]) => void) {
    this.signalRService.registerOnServerEvents(this.hubId, channel, handler);
  }

  public send(channel: string, data?: any) {
    this.signalRService.send(this.hubId, channel, data);
  }

  private init() {
    this.registerChannel('SetConnectionId', (id) => {
      this.connectIdEvent.emit(id);
    });
  }
}    
