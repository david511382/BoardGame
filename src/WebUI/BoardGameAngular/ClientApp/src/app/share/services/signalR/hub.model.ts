import { HubConnection, HubConnectionBuilder , LogLevel, HttpTransportType} from '@aspnet/signalr';
import { EventEmitter } from '@angular/core';

export class SignalRHub {
  public ConnectionEstablished = new EventEmitter<Boolean>();
  public ConnectionErrHandler = new EventEmitter<any>();

  public isRegistered: boolean = false;

  private connectionIsEstablished = false;
  private _hubConnection: HubConnection;

  constructor(url:string) {
    this.createConnection(url);
  }

  public StartConnection(): void {
    this._hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        this.ConnectionEstablished.emit(this.connectionIsEstablished);
      })
      .catch((e) => this.ConnectionErrHandler.emit(e));
  }

  public Send(channel: string, data?: any) {
    if (data !== undefined)
      this._hubConnection.invoke(channel, data);
    else
      this._hubConnection.invoke(channel);
  }

  public RegisterOnServerEvents(channel: string, handler: (...args: any[]) => void): void {
    if (!this.isRegistered) 
      this.isRegistered = true;

    this._hubConnection.on(channel, handler);
  }

  private createConnection(url:string) {
    this._hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Debug)
      .withUrl(url, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();
  }
}    
