import { Injectable, Inject } from '@angular/core';

class BaseUrl {
  constructor(protected readonly baseUrl: string) { }
}

@Injectable()
export class UrlConfigService extends BaseUrl{
  public readonly userBackendUrl: UserUrl
  public readonly roomBackendUrl: RoomUrl
  public readonly gameBackendUrl: GameUrl
  public readonly bigTwoBackendUrl: BigTwoUrl
  
  constructor(@Inject('BASE_URL') baseUrl: string) {
    super(`${baseUrl}api`);
    this.userBackendUrl = new UserUrl(`${this.baseUrl}/User`);
    this.roomBackendUrl = new RoomUrl(`${this.baseUrl}/Room`);
    this.gameBackendUrl = new RoomUrl(`${this.baseUrl}/Game`);
    this.bigTwoBackendUrl = new BigTwoUrl(`${this.baseUrl}/BigTwo`);
  }
}

export class UserUrl extends BaseUrl{
  constructor(baseUrl: string) {
    super(baseUrl);
  }
  
  get UserStatus(): string {
    return `${this.baseUrl}/Status`;
  }
  get Login(): string {
    return `${this.baseUrl}/Login`;
  }
  get Register(): string {
    return `${this.baseUrl}/RegisterAndLogin`;
  }
  get Update(): string {
    return `${this.baseUrl}/update`;
  }
  get Info(): string {
    return `${this.baseUrl}`;
  }
}

export class RoomUrl extends BaseUrl {
  constructor(baseUrl: string) {
    super(baseUrl);
  }

  get Start(): string {
    return `${this.baseUrl}/Start`;
  }
  get List(): string {
    return `${this.baseUrl}`;
  }
  get Create(): string {
    return `${this.baseUrl}`;
  }
  get Join(): string {
    return `${this.baseUrl}`;
  }
  get Leave(): string {
    return `${this.baseUrl}`;
  }
}

export class GameUrl extends BaseUrl {
  constructor(baseUrl: string) {
    super(baseUrl);
  }

  get List(): string {
    return `${this.baseUrl}`;
  }
}

export class BigTwoUrl extends BaseUrl {
  constructor(baseUrl: string) {
    super(baseUrl);
  }

  get HandCards(): string {
    return `${this.baseUrl}/HandCards`;
  }
  get SelectCards(): string {
    return `${this.baseUrl}/SelectCards`;
  }
  get PlayCards(): string {
    return `${this.baseUrl}/PlayCards`;
  }
}
