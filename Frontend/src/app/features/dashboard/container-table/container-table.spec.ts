import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ContainerTable } from './container-table';

describe('ContainerTable', () => {
  let component: ContainerTable;
  let fixture: ComponentFixture<ContainerTable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContainerTable],
    }).compileComponents();

    fixture = TestBed.createComponent(ContainerTable);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
