import { EventEmitter, Injectable } from '@angular/core';
import { ICardResponseModel } from './bigtwo.service';
import { RoomSignalRService } from '../../signalR/room-signalr.service';

@Injectable()
export class BigtwoSignalREventService {
  public connectionId: string;

  gameBoardUpdateEvent = new EventEmitter<ICardResponseModel[]>();
  
  constructor(private signalRService: RoomSignalRService) {
    this.connectionId = this.signalRService.connectionId;
    this.signalRService.connectIdEvent.subscribe((id) => this.connectionId = id);
    this.registerChannel();
  }
  
  private registerChannel() {
    this.signalRService.registerChannel('GameBoardUpdate', (data) => {
      this.gameBoardUpdateEvent.emit(data);
    });
  }
}    
