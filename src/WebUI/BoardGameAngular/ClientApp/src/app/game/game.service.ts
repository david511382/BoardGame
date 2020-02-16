import { Injectable } from '@angular/core';
import { AuthService, UserStatusData } from '../auth/auth.service';

export interface IGameData {
  playerId: number
}
export class GameData implements IGameData {
  constructor(public playerId: number, public tableData: any, public playerData: any) { }
}

@Injectable({
  providedIn: 'root',//singleton 
})
export class GameService {
  public gameId: number;
  public data: GameData;

  constructor(authService: AuthService) {
    authService.userStatusDataChanged.subscribe((data) => this.setData(data));
    var data = authService.userDataBuffer;
    this.setData(data);
  }

  private setData(data: UserStatusData) {
    if (data && data.isInGame) {
      this.gameId = data.room.game.id;
      this.data = new GameData(data.id, data.tableCards, data.playerCards);
    }
  }
}

