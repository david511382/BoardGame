import { Component, ViewChild } from '@angular/core';
import { UserService, UserInfoModel } from '../user.service';
import { Location } from '@angular/common';
import { UserInfoComponent } from '../info/info.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent  {
  @ViewChild(UserInfoComponent, { static: false }) info: UserInfoComponent;

  constructor(private service: UserService,private location :Location) { 
  }
  
  public Register() {
    var request = new UserInfoModel(this.info.name, this.info.username, this.info.password);
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
