import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { Observable } from 'rxjs';
import { Container } from '../models/container';

@Service()
export class ContainerService {
    private http = inject(HttpClient);
    getContainers():Observable<Container[]>{
        return this.http.get<Container[]>('/api/containers')
    }
}
