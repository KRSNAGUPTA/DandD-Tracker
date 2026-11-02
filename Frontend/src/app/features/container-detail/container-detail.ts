import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe, JsonPipe } from '@angular/common';
import { ContainerDetail } from '../../core/models/container';
import { ContainerService } from '../../core/services/containers';

@Component({
  selector: 'app-container-detail',
  standalone: true,
  imports: [RouterLink, DatePipe, JsonPipe],
  templateUrl: './container-detail.html',
  styleUrl: './container-detail.css'
})
export class ContainerDetailPage implements OnInit {
  private route = inject(ActivatedRoute);
  private containerService = inject(ContainerService);

  data = signal<ContainerDetail | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : NaN;

    if (!id || isNaN(id) || id <= 0) {
      this.error.set('Invalid container ID');
      this.loading.set(false);
      return;
    }

    this.containerService.getContainer(id).subscribe({
      next: (d) => {
        this.data.set(d);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(
          err.status === 404
            ? 'Container not found'
            : err.message ?? 'Failed to load container'
        );
        this.loading.set(false);
      }
    });
  }
}