import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',//singleton 
})
export class CommonDataService {
  private data: Map<string, any>;

  constructor() {
    this.data = new Map<string, any>();
  }

  public Set(key: string, value: any){
    return this.data.set(key, value);
  }
  public Get(key: string): any {
    return this.data.get(key);
  }
}    
