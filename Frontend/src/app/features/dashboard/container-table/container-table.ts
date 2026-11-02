import { Component, inject, OnInit, signal } from '@angular/core';
import {Container} from "../../../core/models/container"
import { ContainerService } from '../../../core/services/containers';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
@Component({
  imports: [CommonModule],
  selector: 'app-container-table',
  styleUrl: './container-table.css',
  templateUrl: './container-table.html',
})
export class ContainerTable implements OnInit {
  private containerService = inject(ContainerService);
  private router = inject(Router);
  data = signal<Container[] | null>(null)
  loading = signal(true);
  error = signal("");
  ngOnInit(): void {
    this.containerService.getContainers().subscribe({
      next:(data)=>{
        this.loading.set(false);
        this.data.set(data);
      },
      error: (error)=>{
        this.loading.set(false);
        this.error.set(error);
      }
    })
  }
  goToDetail(id: number): void{
    console.log("Clicked ", id)
    this.router.navigate(['/containers', id]);
  }
}
