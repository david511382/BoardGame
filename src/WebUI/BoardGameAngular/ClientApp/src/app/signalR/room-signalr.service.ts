import { Injectable, EventEmitter } from '@angular/core';
import { SignalRService, HubEnum } from './signalr-manager.service';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class RoomSignalRService {
  public connectIdEvent = new EventEmitter<string>();

  public connectionId: string;
  
  private hubId = HubEnum.GameRoom;

  constructor(
    private signalRService: SignalRService,
    authService: AuthService) {
    this.signalRService.create(HubEnum.GameRoom, "/gameRoomHub");
    this.connectIdEvent.subscribe((id) => this.connectionId = id);
    this.init();

    authService.userStatusDataChanged
      .subscribe((data) => {
        if (data.isInRoom || data.isInGame)
          this.send("GoInGroup", data.room.hostID.toString());
      });
    if (authService.userDataBuffer)
      if (authService.userDataBuffer.isInRoom || authService.userDataBuffer.isInGame)
        this.send("GoInGroup", authService.userDataBuffer.room.hostID.toString());

    this.send("GetConnectionId");
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
