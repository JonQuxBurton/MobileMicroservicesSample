import http from "k6/http";
import { check, group, sleep } from 'k6';
import { Counter } from 'k6/metrics';

let orderMobileData;
let completeProvisionData;
let activateMobileData;
let completeActivateData;

loadData();

let vus = 1;//5;
let iterations = 3;//3;

export let options = {
  scenarios: {
    createCustomerTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'createCustomer',
    },
    orderMobileTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'orderMobile',
    },
    completeProvisionTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'completeProvision',
    },
    activateMobileTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'activateMobile',
    },
    completeActivateTests: {
      executor: 'per-vu-iterations',
      vus: vus,
      iterations: iterations,
      exec: 'completeActivate',
    }
  }
};

const SLEEP_DURATION = 0.1;
const SLEEP_DURATION_BEFORE_ORDER_COMPLETION = 40.0;
const SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK = 20.0;
const RETRIES_FOR_ORDER_COMPLETION_CHECK = 10;

let counters = {
  orderMobile: 0,
  completeProvision: 0,
  activateMobile: 0,
  completeActivate: 0,
  createCustomer: 0
};

let orderMobileErrorMetrics = new Counter("orderMobileErrors");
let completeProvisionErrorMetrics = new Counter("completeProvisionErrors");
let activateMobileErrorMetrics = new Counter("activateMobileErrors");
let completeActivateErrorMetrics = new Counter("completeActivateErrors");

export function createCustomer() {
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };
  group('Create a Customer', (_) => {
    params.tags.name = 'create-customer';
    let customersUrl = "http://localhost:5000/api/customers";
    let createCustomerBody = JSON.stringify({
      Name: `Armstrong-${__VU}-${__ITER} Corporation`
    });

    let createCustomerResponse = http.post(customersUrl, createCustomerBody, params);

    check(createCustomerResponse, {
      'is status 200': (r) => r.status === 200,
      'is customerId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    let customerId = createCustomerResponse.json()['globalId'];
    let getCustomerResponse = httpGetWithRetry(`http://localhost:5000/api/customers/${customerId}`, params);

    check(getCustomerResponse, {
      'is status 200': (r) => r.status === 200
    });
  });
}

export function orderMobile() {
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };
  let data = orderMobileData[counters.orderMobile++];

  group('Order a Mobile', (_) => {
    params.tags.name = 'order-mobile';
    let customerId = data.customerId;
    let phoneNumber = data.phoneNumber;
    let contactName = data.contactName;
    let contactPhoneNumber = data.contactPhoneNumbers;

    let orderMobileUrl = `http://localhost:5000/api/customers/${customerId}/provision`;
    let orderMobileBody = JSON.stringify({
      PhoneNumber: phoneNumber,
      Name: contactName,
      ContactPhoneNumber: contactPhoneNumber
    });

    let orderMobileResponse = http.post(orderMobileUrl, orderMobileBody, params);

    let orderMobileSuccess = check(orderMobileResponse, {
      'when orderMobile, is status 200': (r) => r.status === 200,
      'when orderMobile, is mobileId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    if (!orderMobileSuccess) {
      let status = orderMobileResponse ? orderMobileResponse.status : "undefined";
      console.log(`FAILED - Request to ${orderMobileResponse.request.url} with returned ${status}`);
      orderMobileErrorMetrics.add(1, { url: orderMobileResponse.request.url });
    }
    sleep(SLEEP_DURATION);

    params.tags.name = 'get-mobile';
    let mobileId = orderMobileResponse.json()['globalId'];
    let getMobileUrl = `http://localhost:5000/api/mobiles/${mobileId}`;
    let getMobileResponse = http.get(getMobileUrl, params);

    let getMobileSuccess = check(getMobileResponse, {
      'when getMobile, is status 200': (r) => r.status === 200,
      'when getMobile, is provisionOrderId present': (r) => r.json('orderHistory.0').hasOwnProperty('globalId'),
    });
    if (!getMobileSuccess) {
      let status = getMobileResponse ? getMobileResponse.status : "undefined";
      console.log(`FAILED - Request to ${getMobileResponse.request.method} ${getMobileResponse.request.url} returned status ${status}`);
      orderMobileErrorMetrics.add(1, { url: getMobileResponse.request.url });
    }

    let provisionOrderId = getMobileResponse.json('orderHistory.0')['globalId'];
    
    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Check whether the Order has been received by the External Service
    let provisionOrderReceivedResponse = httpGetWithRetry(`http://localhost:5001/api/orders/${provisionOrderId}`, params);
    let provisionOrderReceivedSuccess = check(provisionOrderReceivedResponse, {
      'when checking provisionOrderReceived, is status 200': (r) => r.status === 200,
    });
    if (!provisionOrderReceivedSuccess) {
      let status = provisionOrderReceivedResponse ? provisionOrderReceivedResponse.status : "undefined";
      console.log(`FAILED - Request to ${provisionOrderReceivedResponse.request.method} ${provisionOrderReceivedResponse.request.url} returned status ${status}`);
      orderMobileErrorMetrics.add(1, { url: provisionOrderReceivedResponse.request.url });
    }
  });
}

export function completeProvision() {
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };
  let data = completeProvisionData[counters.completeProvision++];

  group('The External Service has completed a Mobile Provision Order', (_) => {
    // Step 3 - The External Service has completed the Mobile Provision Order
    params.tags.name = 'complete-provision';
    let mobileId = data.mobileId;
    let provisionOrderId = data.provisionOrderId;
    let completeProvisionUrl = `http://localhost:5001/api/orders/${provisionOrderId}/complete`;

    let completeProvisionResponse = http.post(completeProvisionUrl, "", params);
    sleep(SLEEP_DURATION);

    let completeProvisionResponseSuccess = check(completeProvisionResponse, {
      'when checking completeProvision, is status 200': (r) => r.status === 200,
      'when checking completeProvision, is activationCode present': (r) => r.json().hasOwnProperty('activationCode'),
    });
    if (!completeProvisionResponseSuccess) {
      let status = completeProvisionResponse ? completeProvisionResponse.status : "undefined";
      console.log(`FAILED - Request to ${completeProvisionResponseSuccess.request.method} ${completeProvisionResponseSuccess.request.url} returned status ${status}`);
      completeProvisionErrorMetrics.add(1, { url: completeProvisionResponseSuccess.request.url });
    }

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Wait unitl the Order has been processed
    let waitingForActivateCheckResponse = waitUntilMobileInState(`http://localhost:5000/api/mobiles/${mobileId}`, params, 'WaitingForActivate');

    if (!waitingForActivateCheckResponse){
      console.log(`FAILED - waitUntilMobileInState http://localhost:5000/api/mobiles/${mobileId} with state 'WaitingForActivate'`);
    }

    let waitingForActivateSuccess = check(waitingForActivateCheckResponse, {
      'when checking is waitingForActivate, is status 200': (r) => r.status === 200
    });
    if (!waitingForActivateSuccess) {
      let status = waitingForActivateCheckResponse ? waitingForActivateCheckResponse.status : "undefined";
      console.log(`FAILED - Request to ${waitingForActivateCheckResponse.request.method} ${waitingForActivateCheckResponse.request.url} returned status ${status}`);
      completeProvisionErrorMetrics.add(1, { url: waitingForActivateCheckResponse.request.url });
    }
  });

}

export function activateMobile() {
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };

  let data = activateMobileData[counters.activateMobile++]

  group('Activate a Mobile', (_) => {
    // Step 4 - Activate a Mobile
    params.tags.name = 'activate-mobile';
    let mobileId = data.mobileId;
    let activationCode = data.activationCode;

    let activateMobileUrl = `http://localhost:5000/api/mobiles/${mobileId}/activate`;
    let activateMobileBody = JSON.stringify({
      ActivationCode: activationCode
    });

    let activateMobileResponse = http.post(activateMobileUrl, activateMobileBody, params);

    let activateMobileSuccess = check(activateMobileResponse, {
      'when activateMobile, is status 200': (r) => r.status === 200,
      'when activateMobile, is activateOrderId present': (r) => r.json().hasOwnProperty('globalId'),
    });
    if (!activateMobileSuccess) {
      let status = activateMobileResponse ? activateMobileResponse.status : "undefined";
      console.log(`FAILED - Request to ${activateMobileResponse.request.method} ${activateMobileResponse.request.url} returned status ${status}`);
      activateMobileErrorMetrics.add(1, { url: activateMobileResponse.request.url });
    }

    let activateOrderId = activateMobileResponse.json()['globalId'];

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);

    // Check whether the Order has been received by the External Service
    let activateOrderReceivedResponse = httpGetWithRetry(`http://localhost:5002/api/orders/${activateOrderId}`, params);
    let activateOrderReceivedSuccess = check(activateOrderReceivedResponse, {
      'when activateOrderReceived, is status 200 2': (r) => r.status === 200
    });
    if (!activateOrderReceivedSuccess) {
      let status = activateOrderReceivedResponse ? activateOrderReceivedResponse.status : "undefined";
      console.log(`FAILED - Request to ${activateOrderReceivedResponse.request.method} ${activateOrderReceivedResponse.request.url} returned status ${status}`);
      activateMobileErrorMetrics.add(1, { url: activateOrderReceivedResponse.request.url });
    }
  });

}

export function completeActivate() {
  let params = {
    headers: {
      'Content-Type': 'application/json',
    },
    tags: {}
  };
  let data = completeActivateData[counters.completeActivate++]

  group('The External Service has completed a Mobile Activate Order', (_) => {

    // Step  5 - The External Service has completed the Mobile Activate Order
    params.tags.name = 'complete-activate';
    let mobileId = data.mobileId;
    let activateOrderId = data.activateOrderId;

    let completeActivateUrl = `http://localhost:5002/api/orders/${activateOrderId}/complete`;

    let completeActivateResponse = http.post(completeActivateUrl, null, params);
    console.log(`completeActivateUrl: {completeActivateUrl}`);

    let completeActivateSuccess = check(completeActivateResponse, {
      'when checking completeActivate, is status 200': (r) => r.status === 200
    });
    if (!completeActivateSuccess) {
      let status = completeActivateResponse ? completeActivateResponse.status : "undefined";
      console.log(`FAILED - Request to ${completeActivateResponse.request.method} ${completeActivateResponse.request.url} returned status ${status}`);
      completeActivateErrorMetrics.add(1, { url: completeActivateResponse.request.url });
    }

    sleep(SLEEP_DURATION_BEFORE_ORDER_COMPLETION);
    
    // Wait unitl the Order has been processed
    let waitingForLiveCheckResponse = waitUntilMobileInState(`http://localhost:5000/api/mobiles/${mobileId}`, params, 'Live');

    if (!waitingForLiveCheckResponse){
      console.log(`FAILED - waitUntilMobileInState http://localhost:5000/api/mobiles/${mobileId} with state 'Live'`);
    }

    let waitingForLiveCheckSuccess = check(waitingForLiveCheckResponse, {
      "when checking waitingForLive, is Mobile in state 'Live'": (r) => r !== null,
      'when checking waitingForLive, is status 200': (r) => r.status === 200
    });
    if (!waitingForLiveCheckSuccess) {
      let status = waitingForLiveCheckResponse ? waitingForLiveCheckResponse.status : "undefined";
      console.log(`FAILED - Request to ${waitingForLiveCheckResponse.request.method} ${waitingForLiveCheckResponse.request.url} returned status ${status}`);
      completeActivateErrorMetrics.add(1, { url: waitingForLiveCheckResponse.request.url });
    }
  });
}

function loadData() {
  const data = JSON.parse(open("./data.json"));
  
  orderMobileData = data.orderMobile;
  completeProvisionData = data.completeProvision;
  activateMobileData = data.activateMobile;
  completeActivateData = data.completeActivate;
}

function httpGetWithRetry(url, params) {
  var res;
  for (var retries = RETRIES_FOR_ORDER_COMPLETION_CHECK; retries > 0; retries--) {
    res = http.get(url, params)
    if (res.status != 404 && res.status != 408 && res.status < 500) {
      return res;
    }
    sleep(SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK);
  }
  return res;
}

function waitUntilMobileInState(url, params, state) {
  let res;
  for (var retries = RETRIES_FOR_ORDER_COMPLETION_CHECK; retries > 0; retries--) {
    res = http.get(url, params)
    if (res.status != 404 && res.status != 408 && res.status < 500 && res.json()['state'] === state) {
      return res;
    }
    sleep(SLEEP_DURATION_FOR_ORDER_COMPLETION_CHECK);
  }
  return null;
}