import { EventEmitter } from "@angular/core";

export interface SetItemComponent {
  Init(data: any);
  ClickEvent: EventEmitter<any>;
  DoubleClickEvent: EventEmitter<any>;
}
