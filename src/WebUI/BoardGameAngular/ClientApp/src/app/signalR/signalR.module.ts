import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { RoomSignalRService } from './room-signalr.service';
import { SignalRService } from './signalr-manager.service';

@NgModule({
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule
  ],
  providers: [
    SignalRService,
    RoomSignalRService
  ],
})
export class SignalRModule { }
