import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Tariffs } from './tariffs';

describe('Tariffs', () => {
  let component: Tariffs;
  let fixture: ComponentFixture<Tariffs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Tariffs],
    }).compileComponents();

    fixture = TestBed.createComponent(Tariffs);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
