import { Component, ViewChild } from '@angular/core';
import { UserService, LoginRequest } from '../user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent  {
  public username: string;
  public password: string;

  constructor(private service: UserService) {
    this.username = "";
    this.password = "";
  }
  
  public Login() {
    var request = new LoginRequest(this.username, this.password);
    var ob = this.service.Login(request);
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        alert(`${resp.name}登入成功\n${resp.message}`);
      });
  }

  public Register() {
    this.service.Register();
  }  
}
