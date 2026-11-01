import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EventTimeline } from './event-timeline';

describe('EventTimeline', () => {
  let component: EventTimeline;
  let fixture: ComponentFixture<EventTimeline>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EventTimeline],
    }).compileComponents();

    fixture = TestBed.createComponent(EventTimeline);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
