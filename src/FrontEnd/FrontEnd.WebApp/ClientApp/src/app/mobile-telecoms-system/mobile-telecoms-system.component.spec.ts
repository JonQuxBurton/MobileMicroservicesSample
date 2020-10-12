import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileTelecomsSystemComponent } from './mobile-telecoms-system.component';

describe('MobileTelecomsSystemComponent', () => {
  let component: MobileTelecomsSystemComponent;
  let fixture: ComponentFixture<MobileTelecomsSystemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobileTelecomsSystemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobileTelecomsSystemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
