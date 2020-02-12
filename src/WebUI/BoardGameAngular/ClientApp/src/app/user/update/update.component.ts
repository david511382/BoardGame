import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { UserService, UserInfoRequest } from '../user.service';
import { UserInfoComponent } from '../info/info.component';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.css'],
})
export class UpdateComponent implements AfterViewInit {
  @ViewChild(UserInfoComponent) userInfo: UserInfoComponent;

  constructor(private service: UserService) {}

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.userInfo.name = this.service.userInfo.Name;
      this.userInfo.username = this.service.userInfo.Username;
      this.userInfo.password = null;
    },0);
  }
  
  public Update() {
    var request = new UserInfoRequest(this.userInfo.name, this.userInfo.username, this.userInfo.password);
    var ob = this.service.Update(request);
    if (ob)
      ob.subscribe((resp) => {
        if (resp.isError) {
          alert(resp.errorMessage);
          return;
        }

        alert(resp.message);
      });
  }
}
