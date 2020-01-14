import { EventEmitter, Injectable } from '@angular/core';
import { SignalRService } from '../../share/services/signalr.service';
import { RoomModel } from './room.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class RoomSignalRService {
  public ConnectionId: string;

  RoomOpened = new EventEmitter();
  RoomPlayerChanged = new EventEmitter<RoomModel>();
  RoomStarted = new EventEmitter();
  RoomClose = new EventEmitter();

  ConnectionEstablished = new EventEmitter<Boolean>();
  
  constructor(private signalRService: SignalRService) {
    var url = "http://localhost:52507/roomHub";
    url = "/roomHub";
    signalRService.CreateConnection(url);

    this.registerChannel();
    this.startConnection();
  }
  
  private registerChannel() {
    this.signalRService.RegisterOnServerEvents('SetConnectionId', (id) => {
      this.ConnectionId = id
    });
    this.signalRService.RegisterOnServerEvents('RoomOpened', () => {
      this.RoomOpened.emit();
    });
    this.signalRService.RegisterOnServerEvents('RoomPlayerChanged', (roomData: RoomModel) => {
      this.RoomPlayerChanged.emit(roomData);
    });
    this.signalRService.RegisterOnServerEvents('RoomStarted', () => {
      this.RoomStarted.emit();
    });
    this.signalRService.RegisterOnServerEvents('RoomClose', () => {
      this.RoomClose.emit();
    });
  }

  private startConnection() {
    var errHandler = function () { this.startConnection; };
    var notifier = this.ConnectionEstablished;
    this.signalRService.StartConnection(() => {
      console.log('Hub connection started');
      notifier.emit(true);
      this.signalRService.Send("GetConnectionId");
    },
      (err) => {
        console.log(err);
        console.log('Error while establishing connection, retrying...');
        setTimeout(errHandler, 5000);
      });
  }
}    
