import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChargeBreakdown } from './charge-breakdown';

describe('ChargeBreakdown', () => {
  let component: ChargeBreakdown;
  let fixture: ComponentFixture<ChargeBreakdown>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChargeBreakdown],
    }).compileComponents();

    fixture = TestBed.createComponent(ChargeBreakdown);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
