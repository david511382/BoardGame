
export class UserModel {
  constructor(public id: number, public name: string, public username: string) { }
}

export class GameModel {
  constructor(
    public id: number,
    public name: string,
    public description: string,
    public maxPlayerCount: number,
    public minPlayerCount: number) { }
}

export class RoomModel {
  constructor(public hostID: number, public game: GameModel, public players: UserModel[]) { }
}
