import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RecordEventDialog } from './record-event-dialog';

describe('RecordEventDialog', () => {
  let component: RecordEventDialog;
  let fixture: ComponentFixture<RecordEventDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecordEventDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(RecordEventDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
