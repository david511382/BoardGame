import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { BigtwoComponent } from './bigtwo/bigTwo.component';
import { GameMainComponent } from './game.component';

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
    ReactiveFormsModule
  ],
  entryComponents: [
    BigtwoComponent,
  ]
})
export class GameModule { }
