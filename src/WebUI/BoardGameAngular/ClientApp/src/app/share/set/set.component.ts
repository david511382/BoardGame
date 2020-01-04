import { Component, Input, Output, EventEmitter, ElementRef, OnInit, ViewChild, ViewContainerRef, Type, ComponentFactoryResolver, Inject, AfterViewInit} from '@angular/core';
import { ViewContainerDirective } from '../view-container/view-container.directive';
import { SetItemComponent } from './set-item.component';

@Component({
  selector: 'set',
  template: `<ng-template view-container></ng-template>`,
})
export class SetComponent implements AfterViewInit{
  @ViewChild(ViewContainerDirective, { static: false }) setHost: ViewContainerDirective;

  @Output() ItemClickedEvent = new EventEmitter();
  @Output() ItemDoubleClickedEvent = new EventEmitter();

  private get viewContainerRef(): ViewContainerRef{
    return this.setHost.viewContainerRef;
  }

  constructor(@Inject(ComponentFactoryResolver) private factoryResolver) {}

  ngAfterViewInit(): void {
    this.Clear();
  }

  public Add(type: Type<SetItemComponent>, data?: any): SetItemComponent {
    const factory = this.factoryResolver
      .resolveComponentFactory(type);

    const component = this.viewContainerRef.createComponent(factory) ;

    const result = (<SetItemComponent>component.instance);
    result.Init(data);
    result.ClickEvent.subscribe((data) => this.ItemClickedEvent.emit(data));
    result.DoubleClickEvent.subscribe((data) => this.ItemDoubleClickedEvent.emit(data));
    
    return result;
  }
  
  public Delete(index: number) {
    this.viewContainerRef.remove(index);
  }

  public Clear() {
    this.viewContainerRef.clear();
  }
}
