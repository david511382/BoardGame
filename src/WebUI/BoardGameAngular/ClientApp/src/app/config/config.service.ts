import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment'

class BaseUrl {
  constructor(protected readonly baseUrl: string) { }
}

@Injectable()
export class UrlConfigService extends BaseUrl{
  public readonly userBackendUrl: UserUrl

  constructor() {
    super(`${environment.backendURL}/api`);
    
    this.userBackendUrl = new UserUrl(`${this.baseUrl}/User`, "Login", "Register", "Update");
  }
}

export class UserUrl extends BaseUrl{
  constructor(baseUrl: string, private login: string, private register: string, private update: string) {
    super(baseUrl);
  }

  get Login(): string {
    return `${this.baseUrl}/${this.login}`;
  }
  get Register(): string {
    return `${this.baseUrl}/${this.register}`;
  }
  get Update(): string {
    return `${this.baseUrl}/${this.update}`;
  }
}
