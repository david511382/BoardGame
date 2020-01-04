import {ViewContainerRef,Directive, TemplateRef} from '@angular/core'

@Directive({
  selector: '[view-container]',
})
export class ViewContainerDirective {
  constructor(public viewContainerRef: ViewContainerRef, public templateRef: TemplateRef<any>) { }
}
