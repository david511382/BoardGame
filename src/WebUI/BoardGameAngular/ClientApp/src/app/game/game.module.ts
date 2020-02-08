import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { GameMainComponent } from './game.component';
import { BigtwoModule } from './bigtwo/bigtwo.module';
import { GameService } from './game.service';

@NgModule({
  declarations: [
    GameMainComponent,
  ],
  exports: [
  ],
  providers: [
    GameService
  ],
  imports: [
    HttpModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule,
    BigtwoModule
  ],
  entryComponents: [
  ]
})
export class GameModule { }
