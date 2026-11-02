import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { DashboardSummary } from '../models/dashboard-summary';

@Service()
export class DashboardService {
    private http = inject(HttpClient);
    getSummary(): Observable<DashboardSummary>{
        return this.http.get<DashboardSummary>('/api/dashboard/summary'); //.pipe(tap(data=>console.log(data)));
    }
}
