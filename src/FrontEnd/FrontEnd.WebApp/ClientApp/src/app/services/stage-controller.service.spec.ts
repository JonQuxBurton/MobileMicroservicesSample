import { TestBed } from '@angular/core/testing';

import { StageControllerService } from './stage-controller.service';

describe('StageControllerService', () => {
  let service: StageControllerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StageControllerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
