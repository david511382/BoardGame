import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { LoginComponent } from './login/login.component';
import { ShareModule } from '../share/share.module';
import { UserService } from './user.service';
import { CookieService } from 'ngx-cookie-service';
import { HttpModule } from '@angular/http';
import { UrlConfigService } from '../config/config.service';
import { RegisterComponent } from './register/register.component';
import { UserInfoComponent } from './info/info.component';

@NgModule({
  declarations: [
    UserInfoComponent,
    LoginComponent,
    RegisterComponent,
  ],
  exports: [
    UserInfoComponent,
    LoginComponent,
    RegisterComponent,
  ],
  providers: [
    UrlConfigService,
    CookieService,
    UserService
  ],
  imports: [
    HttpModule,
    ShareModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule
  ],
})
export class UserModule { }
