import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { BigtwoComponent } from './bigtwo/bigtwo.component';
import { GameMainComponent } from './game.component';
import { ShareModule } from './share/share.module'

@NgModule({
  declarations: [
    GameMainComponent,
    BigtwoComponent,
  ],
  exports: [
    GameMainComponent,
  ],
  providers: [
  ],
  imports: [
    HttpModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule,
    ShareModule
  ],
  entryComponents: [
    BigtwoComponent,
  ]
})
export class GameModule { }
