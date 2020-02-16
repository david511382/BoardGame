import { Component, ViewChild, TemplateRef } from '@angular/core';
import { GameService } from './game.service';

enum gameEnum {
  BigTwo=2
}

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
})
export class GameMainComponent{
  @ViewChild('Loading', { static: true }) LoadingPage: TemplateRef<any>;
  @ViewChild('BigTwo', { static: true }) BigTwoPage: TemplateRef<any>;

  public displayPage: TemplateRef<any>;
  public contextData = {
    $implicit: 'Loading...', gameData: null
  };
  
  constructor(private service: GameService) {
    setTimeout(() => this.init(), 0);
  }

  private init() {
    this.contextData.gameData = this.service.data;
    this.showGame();
  }

  private showGame() {
    var gameId = this.service.gameId;
    switch (gameId as gameEnum) {
      case gameEnum.BigTwo:
        this.displayPage = this.BigTwoPage;
        break;
      default:
        console.error(`undefind game id ${gameId}`);
        this.displayPage = this.LoadingPage;
    }
  }
}
