import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { TextBoxComponent } from './text-box/text-box.component';
import { SetComponent } from './set/set.component';
import { ViewContainerDirective } from './view-container/view-container.directive';
import { ToggleButtonComponent } from './toggle-button/toggle-button.component';

@NgModule({
  declarations: [
    ToggleButtonComponent,
    TextBoxComponent,
    SetComponent,
    ViewContainerDirective,
  ],
  exports: [
    ToggleButtonComponent,
    TextBoxComponent,
    SetComponent,
    ViewContainerDirective,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule
  ],
  providers: [
  ],
  entryComponents: [
    TextBoxComponent,
    SetComponent,
    ]
})
export class ShareModule { }
