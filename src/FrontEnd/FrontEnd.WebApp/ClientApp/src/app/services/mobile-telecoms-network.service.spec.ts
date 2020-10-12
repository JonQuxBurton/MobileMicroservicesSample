import { TestBed } from '@angular/core/testing';

import { MobileTelecomsNetworkService } from './mobile-telecoms-network.service';

describe('MobileTelecomsNetworkService', () => {
  let service: MobileTelecomsNetworkService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MobileTelecomsNetworkService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
