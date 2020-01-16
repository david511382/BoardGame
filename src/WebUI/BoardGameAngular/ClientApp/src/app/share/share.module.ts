import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { TextBoxComponent } from './text-box/text-box.component';
import { SetComponent } from './set/set.component';
import { ViewContainerDirective } from './view-container/view-container.directive';
import { SignalRService } from './services/signalR/signalr-manager.service';
import { CommonDataService } from './services/common-data/common-data.service';

@NgModule({
  declarations: [
    TextBoxComponent,
    SetComponent,
    ViewContainerDirective,
  ],
  exports: [
    TextBoxComponent,
    SetComponent,
    ViewContainerDirective,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule
  ],
  providers: [
    SignalRService,
    CommonDataService
  ],
  entryComponents: [
    TextBoxComponent,
    SetComponent,
    ]
})
export class ShareModule { }
