import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AlertPanel } from './alert-panel';

describe('AlertPanel', () => {
  let component: AlertPanel;
  let fixture: ComponentFixture<AlertPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AlertPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(AlertPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
