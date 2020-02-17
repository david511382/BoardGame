import { Component, Input, Output, EventEmitter} from '@angular/core';

@Component({
  selector: 'toggle-button',
  templateUrl: './toggle-button.component.html',
  styleUrls: ['./toggle-button.component.css']
})
export class ToggleButtonComponent {
  @Input() text: string;

  @Output() valueChange = new EventEmitter<boolean>();

  public isToggle: boolean;
  public get isActive(): boolean {
    return this.enable && this.isToggle;
  }

  private enable: boolean;

  constructor() {
    this.enable = true;
    this.isToggle = false;
  }

  public setEnable(e: boolean) {
    this.enable = e;
    this.valueChange.emit(this.isActive);
  }

  public setToggle(e: boolean) {
    this.isToggle = e;
    this.valueChange.emit(this.isActive);
  }

  public click(e: any) {
    this.isToggle = !this.isToggle;
    this.valueChange.emit(this.isActive);
  }
}
