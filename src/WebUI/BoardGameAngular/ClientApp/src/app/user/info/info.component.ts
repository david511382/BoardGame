import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'user-info',
  templateUrl: './info.component.html',
  styleUrls: ['./info.component.css'],
})
export class UserInfoComponent{
  @Input() IsUsingName: boolean;
  @Input() ButtonText: string;
  @Output() ButtonClick = new EventEmitter();

  public name: string;
  public username: string;
  public password: string;

  constructor() {
    this.name = "";
    this.username = "";
    this.password = "";
    this.ButtonText = "按鈕";
    this.IsUsingName = true;
  }

  public ClickButton() {
    this.ButtonClick.emit();
  }
}
