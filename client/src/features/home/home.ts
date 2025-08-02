import { Component, Input, signal } from '@angular/core';
import { Register } from "../account/register/register";

@Component({
  selector: 'app-home',
  imports: [Register],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  protected registerMode = signal(false); //Toggle true or false to update Register Form

  showRegister(value: boolean) {
    this.registerMode.set(value);
  }
}
