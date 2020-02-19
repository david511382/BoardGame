import { EventEmitter, Injectable } from '@angular/core';
import { RoomSignalRService } from '../../signalR/room-signalr.service';

export interface ICardResponseModel {
  number: number;
  suit: number;
}
export interface IConditionModel {
  turnId: number;
  winPlayerId: number;
}
export interface IGameBoardModel {
  cards: ICardResponseModel[];
  condition: IConditionModel;
}

@Injectable()
export class BigtwoSignalREventService {
  public connectionId: string;

  gameBoardUpdateEvent = new EventEmitter<IGameBoardModel>();
  
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
