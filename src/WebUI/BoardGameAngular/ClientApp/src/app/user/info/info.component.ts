import { Component, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, AbstractControl } from '@angular/forms';

@Component({
  selector: 'user-info',
  templateUrl: './info.component.html',
  styleUrls: ['./info.component.css'],
})
export class UserInfoComponent{
  @Input() ButtonText: string;
  @Output() ButtonClick = new EventEmitter();
  
  public get name(): string {
    return this.form.get("name").value as string;
  }
  public set name(value: string ){
    this.form.patchValue({
      name: value,
    });
  }
  @Input()
  public IsContainName: boolean;
  @Input()
  public get isNameRequired():boolean {
    return this.isFormRequired("name");
  }
  public set isNameRequired(value: boolean) {
    this.setFormRequired("name", value);
  }

  public get username(): string {
    return this.form.get("username").value as string;
  }
  public set username(value: string) {
    this.form.patchValue({
      username: value,
    });
  }
  @Input()
  public get isUsernameRequired(): boolean {
    return this.isFormRequired("username");
  }
  public set isUsernameRequired(value: boolean) {
    this.setFormRequired("username", value);
  }

  public get password(): string {
    return this.form.get("password").value as string;
  }
  public set password(value: string) {
    this.form.patchValue({
      password: value,
    });
  }
  @Input()
  public get isPasswordRequired(): boolean {
    return this.isFormRequired("password");
  }
  public set isPasswordRequired(value: boolean) {
    this.setFormRequired("password", value);
  }
  @Input() IsPasswordShowable: boolean;

  form = this.formBuilder.group({
    name: [''],
    username: [''],
    password: ['']
  })

  private isShowPassword: boolean;
  get passwordType(): string {
    if (this.isShowPassword)
      return "text";
    return "password";
  }

  constructor(private formBuilder : FormBuilder ) {
    this.ButtonText = "按鈕";
    this.IsContainName = true;
    this.isShowPassword = false;
    this.IsPasswordShowable = true;
  }

  public ClickButton() {
    this.ButtonClick.emit();
  }

  private isFormRequired(name: string): boolean {
    var abstractControl = this.form.get(name);
    if (abstractControl.validator) {
      const validator = abstractControl.validator({} as AbstractControl);
      if (validator && validator.required) {
        return true;
      }
    }
    return false;
  }
  private setFormRequired(name: string, isRequired:boolean) {
    var target = this.form.get(name);
    if (isRequired)
      target.setValidators([Validators.required]);
    //target.setValidators([Validators.required, Validators.minLength(2)]);
    else
      target.clearValidators();
  }
}
