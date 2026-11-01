import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectionPicker } from './projection-picker';

describe('ProjectionPicker', () => {
  let component: ProjectionPicker;
  let fixture: ComponentFixture<ProjectionPicker>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectionPicker],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectionPicker);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
