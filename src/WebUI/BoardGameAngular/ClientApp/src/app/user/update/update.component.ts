import { Component, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { UserService, UserInfoModel } from '../user.service';
import { UserInfoComponent } from '../info/info.component';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css'],
})
export class UpdateComponent {
  @ViewChild(UserInfoComponent, { static: false }) UserInfo: UserInfoComponent;

  constructor(private service: UserService) {
    setTimeout(() => this.onLoad(), 0);
  }

  private onLoad() {
    this.UserInfo.name = this.service.UserInfo.Name;
    this.UserInfo.username = this.service.UserInfo.Username;
    this.UserInfo.password = this.service.UserInfo.Password;
  }

  public Update() {
    var request = new UserInfoModel(this.UserInfo.name, this.UserInfo.username, this.UserInfo.password);
    var ob = this.service.Update(request);
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage)
          return;
        }

        alert(resp.message);
      });
  }
}
