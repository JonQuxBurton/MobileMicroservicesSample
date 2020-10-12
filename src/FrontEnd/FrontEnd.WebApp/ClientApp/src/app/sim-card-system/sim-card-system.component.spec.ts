import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SimCardSystemComponent } from './sim-card-system.component';

describe('SimCardSystemComponent', () => {
  let component: SimCardSystemComponent;
  let fixture: ComponentFixture<SimCardSystemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SimCardSystemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SimCardSystemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
