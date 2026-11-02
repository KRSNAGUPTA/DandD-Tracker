import { TestBed } from '@angular/core/testing';
import { Containers } from './containers';

describe('Containers', () => {
  let service: Containers;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Containers);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
