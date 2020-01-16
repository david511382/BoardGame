import { Injectable } from '@angular/core';
import { SignalRHub } from './hub.model';

export enum HubEnum {
  GameRoom
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class SignalRService {
  private _hubs: Map<HubEnum, SignalRHub>;

  constructor() {
    this._hubs = new Map<HubEnum, SignalRHub>();

    var gameRoomUrl = "/gameRoomHub";
    var gameRoomHub = new SignalRHub(gameRoomUrl);
    gameRoomHub.ConnectionErrHandler.subscribe((err) => this.connectionErrHandler(gameRoomHub, err));
    gameRoomHub.ConnectionEstablished.subscribe(() => this.startConnectionHandler(gameRoomHub));
    this._hubs.set(HubEnum.GameRoom, gameRoomHub);
  }

  public Send(hubId: HubEnum, channel: string, data?: any) {
    var hub = this._hubs.get(hubId);
    hub.Send(channel, data);
  }

  public RegisterOnServerEvents(hubId: HubEnum, channel: string, handler: (...args: any[]) => void): void {
    var hub = this._hubs.get(hubId);
    var isFirstRegistered = !hub.isRegistered;

    hub.RegisterOnServerEvents(channel, handler);

    if (isFirstRegistered)
      hub.StartConnection();
  }

  private startConnectionHandler(hub: SignalRHub) {
    console.log('Hub connection started');
    hub.Send("GetConnectionId");
  }

  private connectionErrHandler(hub: SignalRHub, err: any) {
    console.log(err);
    console.log('Error while establishing connection, retrying...');
    setTimeout(()=>hub.StartConnection(), 5000);
  }
}    
