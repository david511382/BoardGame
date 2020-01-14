import { HubConnection, HubConnectionBuilder , LogLevel, HttpTransportType} from '@aspnet/signalr';
import { Injectable } from '@angular/core';

@Injectable()
export class SignalRService {
  private connectionIsEstablished = false;
  private _hubConnection: HubConnection;
  
  public Send(channel: string, data?: any) {
    if (data!== undefined)
      this._hubConnection.invoke(channel, data);
    this._hubConnection.invoke(channel);
  }

  public RegisterOnServerEvents(channel: string, handler: (...args: any[]) => void): void {
    this._hubConnection.on(channel, handler);
  }

  public CreateConnection(url:string) {
    this._hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Debug)
      .withUrl(url, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();
  }

  public StartConnection(startConnectHandler: () => void, startConnectErrHandler:(err:any)=>void): void {
    this._hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        startConnectHandler();
      })
      .catch(startConnectErrHandler);
  }
}    
