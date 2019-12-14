import { Component } from '@angular/core';
import { UserService, RegisterAndLoginRequest } from '../user.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent  {
  public name: string;
  public username: string;
  public password: string;

  constructor(private service: UserService,private location :Location) {
    this.name = "";
    this.username = "";
    this.password = "";    
  }
  
  public Register() {
    var request = new RegisterAndLoginRequest(this.name, this.username, this.password);
    var ob = this.service.RegisterAndLogin(request);
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        this.location.back();
        alert(`${resp.name}註冊並登入成功\n${resp.message}`);
      });
  }
}
