import { Injectable, Inject } from '@angular/core';

class BaseUrl {
  constructor(protected readonly baseUrl: string) { }
}

@Injectable()
export class UrlConfigService extends BaseUrl{
  public readonly userBackendUrl: UserUrl

  constructor(@Inject('BASE_URL') baseUrl: string) {
    super(`${baseUrl}/api`);
    this.userBackendUrl =  new UserUrl(`${this.baseUrl}/User`, "Login", "Register", "Update");
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
