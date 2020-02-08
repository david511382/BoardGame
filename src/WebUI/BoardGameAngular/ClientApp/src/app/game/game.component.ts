import { Component, ViewChild, TemplateRef } from '@angular/core';
import { CommonDataService } from '../share/services/common-data/common-data.service';

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
  public contextData = { $implicit: 'Loading...', game: 'game' };
  
  constructor(private dataService: CommonDataService) {
    setTimeout(() => this.init(), 0);
  }

  private init() {
    var gameId = this.dataService.Get("gameId");
    this.showGame(gameId);
  }

  private showGame(gameID: number) {
    switch (gameID as gameEnum) {
      case gameEnum.BigTwo:
        this.displayPage = this.BigTwoPage;
        break;
      default:
        console.error(`undefind game id ${gameID}`);
        this.displayPage = this.LoadingPage;
    }
  }
}
