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
  }

  public create(hubEnum: HubEnum, url: string, startHandler: (h: SignalRHub) => void) {
    var hub = this._hubs.get(hubEnum);
    if (hub)
      return;

    hub = new SignalRHub(url);
    hub.ConnectionErrHandler.subscribe((err) => this.connectionErrHandler(hub, err));
    hub.ConnectionEstablished.subscribe(() => this.startConnectionHandler(hub, startHandler));
    this._hubs.set(hubEnum, hub);
  }

  public send(hubId: HubEnum, channel: string, data?: any) {
    var hub = this._hubs.get(hubId);
    hub.Send(channel, data);
  }

  public registerOnServerEvents(hubId: HubEnum, channel: string, handler: (...args: any[]) => void): void {
    var hub = this._hubs.get(hubId);
    var isFirstRegistered = !hub.isRegistered;

    hub.RegisterOnServerEvents(channel, handler);

    if (isFirstRegistered)
      hub.StartConnection();
  }

  private startConnectionHandler(hub: SignalRHub, startHandler: (h: SignalRHub) => void) {
    console.log('Hub connection started');
    startHandler(hub);
  }

  private connectionErrHandler(hub: SignalRHub, err: any) {
    console.log(err);
    console.log('Error while establishing connection, retrying...');
    setTimeout(()=>hub.StartConnection(), 5000);
  }
}    
