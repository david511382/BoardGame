import { Component } from '@angular/core';
import { IRoute } from './route.const';
import { NavMenuService } from './nav-menu.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
  providers: [NavMenuService]
})
export class NavMenuComponent {
  public Routes: IRoute[];
  isExpanded = false;

  constructor(private service: NavMenuService) {
    service.RouteChanged.subscribe((routes)=> this.Routes = routes);
    this.Routes = service.Routes;
  }

  public Click(navName : string) {
    this.service.ClickNav(navName);
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
