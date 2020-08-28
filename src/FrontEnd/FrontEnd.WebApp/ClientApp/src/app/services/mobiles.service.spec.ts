import { TestBed } from '@angular/core/testing';

import { MobilesService } from './mobiles.service';

describe('MobilesService', () => {
  let service: MobilesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MobilesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
