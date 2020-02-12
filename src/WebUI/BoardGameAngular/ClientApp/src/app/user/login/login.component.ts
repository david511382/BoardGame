import { Component, ViewChild } from '@angular/core';
import { UserService, LoginRequest } from '../user.service';
import { Location } from '@angular/common';
import { UserInfoComponent } from '../info/info.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  @ViewChild(UserInfoComponent) info: UserInfoComponent;

  constructor(private service: UserService, private location: Location) {}
  
  public Login() {
    var request = new LoginRequest(this.info.username, this.info.password);
    var ob = this.service.Login(request);
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        this.location.back();
        alert(`${resp.name}登入成功\n${resp.message}`);
      });
  }
}
