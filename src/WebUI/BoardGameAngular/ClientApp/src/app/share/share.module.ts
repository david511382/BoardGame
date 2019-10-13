import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { TextBoxComponent } from './text-box/text-box.component';

@NgModule({
  declarations: [
    TextBoxComponent,
  ],
  exports: [
    TextBoxComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule
  ],
  entryComponents: [TextBoxComponent]
})
export class ShareModule { }
