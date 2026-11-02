import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from './features/header/header';
import { Dashboard } from './features/dashboard/dashboard';

@Component({
  imports: [RouterOutlet, Header, Dashboard],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('D&D Tracker - Econship');
}
