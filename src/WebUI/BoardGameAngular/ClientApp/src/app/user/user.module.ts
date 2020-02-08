import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { LoginComponent } from './login/login.component';
import { ShareModule } from '../share/share.module';
import { UserService } from './user.service';
import { CookieService } from 'ngx-cookie-service';
import { HttpModule } from '@angular/http';
import { UrlConfigService } from '../config/config.service';
import { RegisterComponent } from './register/register.component';
import { UserInfoComponent } from './info/info.component';
import { UpdateComponent } from './update/update.component';
import { AuthService } from '../auth/auth.service';

@NgModule({
  declarations: [
    UserInfoComponent,
    LoginComponent,
    RegisterComponent,
    UpdateComponent,
  ],
  exports: [
    UserInfoComponent,
    LoginComponent,
    RegisterComponent,
    UpdateComponent
  ],
  providers: [
    UrlConfigService,
    CookieService,
    UserService,
    AuthService
  ],
  imports: [
    HttpModule,
    ShareModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    FormsModule,
    ReactiveFormsModule
  ],
})
export class UserModule { }
