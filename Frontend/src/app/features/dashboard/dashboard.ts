import { Component } from '@angular/core';
import { AlertPanel } from './alert-panel/alert-panel';
import { ContainerTable } from './container-table/container-table';
import { SummaryCard } from './summary-card/summary-card';

@Component({
  imports: [AlertPanel, ContainerTable, SummaryCard],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard {}
