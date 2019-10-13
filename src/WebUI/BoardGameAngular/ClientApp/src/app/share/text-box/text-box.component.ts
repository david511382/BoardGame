import { Component, Input, Output, EventEmitter, ElementRef} from '@angular/core';

export enum Style {
  normal = "normal",
  pretty = "pretty"
}

@Component({
  selector: 'text-box',
  templateUrl: './text-box.component.html',
  styleUrls: ['./text-box.component.css']
})
export class TextBoxComponent {
  @Input() id: string;
  @Input() ClassStyle: Style;
  @Input() title: string;
  @Input() widthEM: number;
  @Input() isNotEnter: boolean;
  @Input()
  get value(){
    return this.v;
  }

  set value(val: string) {
    if (!val)
      val = "";

    this.v = val;

    if (this.v.length !== 0)
      this.IsLabelInText = false;
    else if (!this.isFocus)
      this.IsLabelInText = true;

    this.valueChange.emit(this.v);
  }

  @Output() valueChange = new EventEmitter();

  public IsLabelInText: boolean;
  private isFocus: boolean;
  private v: string;

  get class(): string {
    return `text-box-wrap ${this.ClassStyle}`;
  }

  constructor(private elRef: ElementRef) {
    this.id = "textBoxTextBox";
    this.v = "";
    this.IsLabelInText = true;
    this.isFocus = false;
    this.widthEM = 20;
    this.ClassStyle = Style.pretty;
    this.isNotEnter = false;
  }

  public focusOut() {
    this.isFocus = false;

    if (this.value.length === 0) {
      this.IsLabelInText = true;
    }
  }

  public focus() {
    this.isFocus = true;

    this.IsLabelInText = false;
  }

  public focusOnText() {
    this.textbox.focus();
    this.focus();
  }

  private get textbox() {
    return this.elRef.nativeElement.childNodes[0].childNodes[0];
  }
}
