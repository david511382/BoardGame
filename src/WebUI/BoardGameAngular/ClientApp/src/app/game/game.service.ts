import { Injectable } from '@angular/core';
import { AuthService, UserStatusData } from '../auth/auth.service';

@Injectable({
  providedIn: 'root',//singleton 
})
export class GameService {
  public  gameId:number;
  public  tableData: any;
  public playerData: any;

  constructor(authService: AuthService) {
    authService.userStatusDataChanged.subscribe((data) => this.setData(data));
    var data = authService.userDataBuffer;
    this.setData(data);
  }

  private setData(data: UserStatusData) {
    if (data && data.isInGame) {
      this.gameId = data.room.game.id;
      this.tableData = data.tableCards;
      this.playerData = data.playerCards;
    }
  }
}

