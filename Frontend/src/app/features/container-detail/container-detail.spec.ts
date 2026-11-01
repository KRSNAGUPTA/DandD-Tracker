import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContainerDetail } from './container-detail';

describe('ContainerDetail', () => {
  let component: ContainerDetail;
  let fixture: ComponentFixture<ContainerDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContainerDetail],
    }).compileComponents();

    fixture = TestBed.createComponent(ContainerDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
