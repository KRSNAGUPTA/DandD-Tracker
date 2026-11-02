import { Component, inject, OnInit, signal } from '@angular/core';
import { AlertPanel } from './alert-panel/alert-panel';
import { ContainerTable } from './container-table/container-table';
import { SummaryCard } from './summary-card/summary-card';
import { DashboardService } from '../../core/services/dashboard';
import { DashboardSummary } from '../../core/models/dashboard-summary';
import { CommonModule } from '@angular/common';

@Component({
  imports: [AlertPanel, ContainerTable, SummaryCard, CommonModule],
  selector: 'app-dashboard',
  styleUrl: './dashboard.css',
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit {
  private dashbaordService = inject(DashboardService);
  summary = signal<DashboardSummary | null>(null);
  loading = signal(true);
  error = signal('');
  ngOnInit(): void {
    this.dashbaordService.getSummary().subscribe({
      next:(data)=>{this.summary.set(data),
        this.loading.set(false);
        this.error.set('');
      },
      error:((err)=>{
        this.loading.set(false);
        this.error.set(err?.message || err);  
      })
      
    })
  }
}
